import java.awt.image.BufferedImage;
import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.HttpURLConnection;
import java.net.InetAddress;
import java.net.Socket;
import java.net.SocketException;
import java.net.SocketTimeoutException;
import java.net.URL;
import javax.imageio.ImageIO;

public class RequestHandler implements Runnable {

    //socket connected to client passed by proxy server
    Socket clientSocket;
    //Read data client sends to proxy
    BufferedReader proxyToClientBuffReader;
    //Send data from proxy to client
    BufferedWriter proxyToClientBuffWriter;

    /**
     * Thread used to transmit data read from client to server when using HTTPS
     */
    private Thread httpsClientToServer;

    /**
     * Creates a ReuqestHandler object capable of servicing HTTP(S) GET requests
     *
     * @param clientSocket socket connected to the client
     */
    public RequestHandler(Socket clientSocket) {
        this.clientSocket = clientSocket;
        try {
            this.clientSocket.setSoTimeout(3000);
            proxyToClientBuffReader = new BufferedReader(new InputStreamReader(clientSocket.getInputStream()));
            proxyToClientBuffWriter = new BufferedWriter(new OutputStreamWriter(clientSocket.getOutputStream()));
        } catch (SocketException e) {
            System.out.println("SocketException occurred");
        } catch (IOException e) {
            System.out.println("IOException occurred");
        }
    }

    /**
     * Reads and examines the requestString and calls the appropriate method based on the request type.
     */
    @Override
    public void run() {
        String requestStr = null; //request from client
        try {
            requestStr = proxyToClientBuffReader.readLine();
        } catch (IOException e) {
            System.out.println("Error reading request from client");
        }

        //parse out url
        System.out.println(String.format("Request received %s", requestStr));
        //get the request type
        String request = requestStr.substring(0, requestStr.indexOf(' '));
        //remove the request type and space
        String urlString = requestStr.substring(requestStr.indexOf(' ') + 1);
        //remove everything past next page
        urlString = urlString.substring(0, urlString.indexOf(' '));

        //prepare http:// if necessary to create correct url
//        if (!urlString.substring(0, 4).equals("http")) {
//            String temp = "http:/";
//            urlString = temp + urlString;
//        }

        if (urlString.charAt(0) == '/') {
            urlString = urlString.substring(1);
            System.out.println(urlString);
        }
        
        //check if site is blocked
        if (Proxy.isBlocked(urlString)) {
            System.out.println(String.format("Blocked site request %s", urlString));
            blockedSiteRequested();
            return;
        }

        //check request type
        if (request.equals("CONNECT")) {
            System.out.println(String.format("HTTPS Request for %s\n", urlString));
            handleHTTPSRequest(urlString);
        } else {
            File file;
            if ((file = Proxy.getCachedPage(urlString)) != null) {
                System.out.println(String.format("Cached copy found: %s\n", urlString));
                sendCachedPageToClient(file);
            } else {
                System.out.println(String.format("HTTP GET for: %s\n", urlString));
                sendNonCachedToClient(urlString);
            }
        }
    }

    /**
     * Sends the specified cached file to the client
     *
     * @param cachedFile The file to be sent (can be image/text)
     */
    private void sendCachedPageToClient(File cachedFile) {
        //Read from file containing cached web page
        try {
            //if file is an img -> write data to client using buffered img
            String fileExtension = cachedFile.getName().substring(cachedFile.getName().lastIndexOf('.'));

            //Response that will be send to server
            String response;
            if ((fileExtension.contains(".png")) || fileExtension.contains(".jpg") || fileExtension.contains(".jpeg")
                || fileExtension.contains(".gif")) {
                //Read img from storage
                BufferedImage image = ImageIO.read(cachedFile);

                if (image == null) {
                    System.out.println(String.format("Image %s was null", cachedFile.getName()));
                    response = "HTTP/1.0 404 NOT FOUND\nProxy-agent: ProxyServer/1.0\\n\n";
                    proxyToClientBuffWriter.write(response);
                    proxyToClientBuffWriter.flush();
                } else {
                    response = "HTTP/1.0 200 OK\nProxy-agent: ProxyServer/1.0\n\n";
                    proxyToClientBuffWriter.write(response);
                    proxyToClientBuffWriter.flush();
                    ImageIO.write(image, fileExtension.substring(1), clientSocket.getOutputStream());
                }
            } else { //standard text based file requested
                BufferedReader cachedFileBuffReader = new BufferedReader(new InputStreamReader(new FileInputStream(cachedFile)));
                response = "HTTP/1.0 200 OK\nProxy-agent: ProxyServer/1.0\n\n";
                proxyToClientBuffWriter.write(response);
                proxyToClientBuffWriter.flush();

                String line;
                while ((line = cachedFileBuffReader.readLine()) != null) {
                    proxyToClientBuffWriter.write(line);
                }
                proxyToClientBuffWriter.flush();

                //close resources
                if (cachedFileBuffReader != null) {
                    cachedFileBuffReader.close();
                }
            }
            //close down resources
            if (proxyToClientBuffWriter != null) {
                proxyToClientBuffWriter.close();
            }
        } catch (IOException e) {
            System.out.println("Error Sending Cached file to client");
        }
    }

    /**
     * Sends the contents of the file specified by the urlString to the client
     *
     * @param urlString URL ofthe file requested
     */
    private void sendNonCachedToClient(String urlString) {
        // Compute a logical file name as per schema
        // This allows the files on stored on disk to resemble that of the URL it was taken from
        int fileExtensionIndex = urlString.lastIndexOf('.');
        String fileExtension;

        //get type of file
        fileExtension = urlString.substring(fileExtensionIndex, urlString.length());

        //get initial file name
        String fileName = urlString.substring(0, fileExtensionIndex);

        //trim of http://www. as no need for it in file name
        fileName = fileName.substring(fileName.indexOf('.') + 1);

        //remove any illegal chars from file name
        fileName = fileName.replace("/", "__");
        fileName = fileName.replace('.', '_');

        //trailing / result in index.html of that directory being fetched
        if (fileExtension.contains("/")) {
            fileExtension = fileExtension.replace("/", "__");
            fileExtension = fileExtension.replace(".", "_");
            fileExtension += ".html";
        }
        fileName = fileName + fileExtension;

        //create File to cache to
        boolean caching = true;
        File fileToCache = null;
        BufferedWriter fileToCacheBuffWriter = null;

        try {
            // Create File to cache
            fileToCache = new File("cached/" + fileName);

            if (!fileToCache.exists()) {
                fileToCache.createNewFile();
            }

            // Create Buffered output stream to write to cached copy of file
            fileToCacheBuffWriter = new BufferedWriter(new FileWriter(fileToCache));
        } catch (IOException e) {
            System.out.println("Couldn't cache: " + fileName);
            caching = false;
            e.printStackTrace();
        } catch (NullPointerException e) {
            System.out.println("NPE opening file");
        }

        try {
            //check if file is an img
            if ((fileExtension.contains(".png")) || fileExtension.contains(".jpg") ||
                fileExtension.contains(".jpeg") || fileExtension.contains(".gif")) {
                //create url
                URL remoteURL = new URL(urlString);
                BufferedImage image = ImageIO.read(remoteURL);

                if (image != null) {
                    // Cache the image to disk
                    ImageIO.write(image, fileExtension.substring(1), fileToCache);

                    // Send response code to client
                    String line = "HTTP/1.0 200 OK\nProxy-agent: ProxyServer/1.0\n\n";
                    proxyToClientBuffWriter.write(line);
                    proxyToClientBuffWriter.flush();

                    // Send them the image data
                    ImageIO.write(image, fileExtension.substring(1), clientSocket.getOutputStream());

                    // No image received from remote server
                } else {
                    System.out.println("Sending 404 to client as image wasn't received from server"
                                           + fileName);
                    String error = "HTTP/1.0 404 NOT FOUND\nProxy-agent: ProxyServer/1.0\n\n";
                    proxyToClientBuffWriter.write(error);
                    proxyToClientBuffWriter.flush();
                }
            } else { //file is a text file
                //create the url
                URL remoteURL = new URL(urlString);
                // Create a connection to remote server
                HttpURLConnection proxyToServerCon = (HttpURLConnection) remoteURL.openConnection();
                proxyToServerCon.setRequestProperty(
                    "Content-Type",
                    "application/x-www-form-urlencoded");
                proxyToServerCon.setRequestProperty("Content-Language", "en-US");
                proxyToServerCon.setUseCaches(false);
                proxyToServerCon.setDoOutput(true);

                // Create Buffered Reader from remote Server
                BufferedReader proxyToServerBR = new BufferedReader(new InputStreamReader(proxyToServerCon.getInputStream()));

                // Send success code to client
                String line = "HTTP/1.0 200 OK\n" +
                    "Proxy-agent: ProxyServer/1.0\n" +
                    "\r\n";
                proxyToClientBuffWriter.write(line);

                // Read from input stream between proxy and remote server
                while ((line = proxyToServerBR.readLine()) != null) {
                    // Send on data to client
                    proxyToClientBuffWriter.write(line);

                    // Write to our cached copy of the file
                    if (caching) {
                        fileToCacheBuffWriter.write(line);
                    }
                }

                // Ensure all data is sent by this point
                proxyToClientBuffWriter.flush();

                // Close Down Resources
                if (proxyToServerBR != null) {
                    proxyToServerBR.close();
                }

                if (caching) {
                    // Ensure data written and add to our cached hash maps
                    fileToCacheBuffWriter.flush();
                    Proxy.addCachedPage(urlString, fileToCache);
                }

                // Close down resources
                if (fileToCacheBuffWriter != null) {
                    fileToCacheBuffWriter.close();
                }

                if (proxyToClientBuffWriter != null) {
                    proxyToClientBuffWriter.close();
                }
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    /**
     * Handles HTTPS requests between client and remote server
     *
     * @param urlString desired file to be transmitted over https
     */
    private void handleHTTPSRequest(String urlString) {
        // Extract the URL and port of remote
        String url = urlString.substring(7);
        String[] pieces = url.split(":");
        url = pieces[0];
        int port = Integer.parseInt(pieces[1]);

        try {
            // Only first line of HTTPS request has been read at this point (CONNECT *)
            // Read (and throw away) the rest of the initial data on the stream
            for (int i = 0; i < 5; i++) {
                proxyToClientBuffReader.readLine();
            }

            // Get actual IP associated with this URL through DNS
            InetAddress address = InetAddress.getByName(url);

            // Open a socket to the remote server
            Socket proxyToServerSocket = new Socket(address, port);
            proxyToServerSocket.setSoTimeout(5000);

            // Send Connection established to the client
            String line = "HTTP/1.0 200 Connection established\r\n" +
                "Proxy-Agent: ProxyServer/1.0\r\n" +
                "\r\n";
            proxyToClientBuffWriter.write(line);
            proxyToClientBuffWriter.flush();

            // Client and Remote will both start sending data to proxy at this point
            // Proxy needs to asynchronously read data from each party and send it to the other party

            //Create a Buffered Writer between proxy and remote
            BufferedWriter proxyToServerBuffWriter =
                new BufferedWriter(new OutputStreamWriter(proxyToServerSocket.getOutputStream()));

            // Create Buffered Reader from proxy and remote
            BufferedReader proxyToServerBuffReader =
                new BufferedReader(new InputStreamReader(proxyToServerSocket.getInputStream()));

            // Create a new thread to listen to client and transmit to server
            ClientToServerHttpsTransmit clientToServerHttps =
                new ClientToServerHttpsTransmit(clientSocket.getInputStream(), proxyToServerSocket.getOutputStream());

            httpsClientToServer = new Thread(clientToServerHttps);
            httpsClientToServer.start();

            // Listen to remote server and relay to client
            try {
                byte[] buffer = new byte[4096];
                int read;
                do {
                    read = proxyToServerSocket.getInputStream().read(buffer);
                    if (read > 0) {
                        clientSocket.getOutputStream().write(buffer, 0, read);
                        if (proxyToServerSocket.getInputStream().available() < 1) {
                            clientSocket.getOutputStream().flush();
                        }
                    }
                } while (read >= 0);
            } catch (SocketTimeoutException ignored) {
            } catch (IOException e) {
                e.printStackTrace();
            }

            // Close Down Resources
            if (proxyToServerSocket != null) {
                proxyToServerSocket.close();
            }

            if (proxyToServerBuffReader != null) {
                proxyToServerBuffReader.close();
            }

            if (proxyToServerBuffWriter != null) {
                proxyToServerBuffWriter.close();
            }

            if (proxyToClientBuffWriter != null) {
                proxyToClientBuffWriter.close();
            }


        } catch (SocketTimeoutException e) {
            String line = "HTTP/1.0 504 Timeout Occured after 10s\n" +
                "User-Agent: ProxyServer/1.0\n" +
                "\r\n";
            try {
                proxyToClientBuffWriter.write(line);
                proxyToClientBuffWriter.flush();
            } catch (IOException ioe) {
                ioe.printStackTrace();
            }
        } catch (Exception e) {
            System.out.println("Error on HTTPS : " + urlString);
            e.printStackTrace();
        }
    }

    /**
     * This method is called when user requests a page that is blocked by the proxy. Sends an access forbidden message back to the
     * client
     */
    private void blockedSiteRequested() {
        try {
            BufferedWriter bufferedWriter = new BufferedWriter(new OutputStreamWriter(clientSocket.getOutputStream()));
            String line = "HTTP/1.0 403 Access Forbidden\nUser-Agent: ProxyServer/1.0\n\n";
            bufferedWriter.write(line);
            bufferedWriter.flush();
        } catch (IOException e) {
            System.out.println("Error writing to client when requested a blocked site");
            e.printStackTrace();
        }
    }
}
