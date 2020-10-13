import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Scanner;

public class Proxy implements Runnable {

    private ServerSocket serverSocket;
    /**
     * semaphore for proxy and console management system
     */
    private volatile boolean running = true;
    /**
     * order lookup of cache items key - url of page request value - file in storage
     */
    static HashMap<String, File> cache;

    static HashMap<String, String> blockedSites;
    /**
     * threads that are currently running and servicing requests
     */
    static ArrayList<Thread> servicingThreads;

    public Proxy(int port) {
        cache = new HashMap<>();
        blockedSites = new HashMap<>();
        servicingThreads = new ArrayList<>();

        //start dynamic manager on a separate thread
        //starts overridden run() method at bottom
        new Thread(this).start();

        try {
            // Load in cached sites from file
            File cachedSites = new File(FileNames.CACHE_FILE.getFileName());
            if (!cachedSites.exists()) {
                System.out.println("No cached sites found.\nCreating new file.");
                cachedSites.createNewFile();
            } else {
                FileInputStream fileInputStream = new FileInputStream(cachedSites);
                ObjectInputStream objectInputStream = new ObjectInputStream(fileInputStream);
                cache = ((HashMap<String, File>) objectInputStream.readObject());
                fileInputStream.close();
                objectInputStream.close();
            }
            // Load in blocked sites from file
            File blockedSitesFile = new File(FileNames.BLOCKED_FILE.getFileName());
            if (!blockedSitesFile.exists()) {
                System.out.println("No blocked sites found.\nCreating new file");
                blockedSitesFile.createNewFile();
            } else {
                FileInputStream fileInputStream = new FileInputStream(blockedSitesFile);
                ObjectInputStream objectInputStream = new ObjectInputStream(fileInputStream);
                blockedSites = ((HashMap<String, String>) objectInputStream.readObject());
                fileInputStream.close();
                objectInputStream.close();
            }
        } catch (IOException e) {
            System.out.println("IOException exception occurred. Error loading previously cached sites file.");
        } catch (ClassNotFoundException e) {
            System.out.println("ClassNotFoundException exception occurred");
        }

        try {
            serverSocket = new ServerSocket(port);
            //set timeout for debug
//            serverSocket.setSoTimeout(100000);
            System.out.println(String.format("Waiting for client on port %d ...", serverSocket.getLocalPort()));
            running = true;
        } catch (IOException e) {
            System.out.println("IOException when connecting to client");
        }
    }

    /**
     * listen to port and accepts new socket connections, creates a new thread to handle request, passes socket connection and
     * continue listening
     */
    public void listen() {
        while (running) {
            try {
                //serverSocket.accept() blocks until a connection is made
                Socket socket = serverSocket.accept();

                // Create new Thread and pass it Runnable RequestHandler
                Thread thread = new Thread(new RequestHandler(socket));

                servicingThreads.add(thread);
                thread.start();

            } catch (IOException e) {
                e.printStackTrace();
            }
        }
    }

    private void closeServer() {
        System.out.println("\nClosing Server..");
        running = false;

        try {
            FileOutputStream fileOutputStreamCache = new FileOutputStream(FileNames.CACHE_FILE.getFileName());
            ObjectOutputStream objectOutputStreamCache = new ObjectOutputStream(fileOutputStreamCache);

            objectOutputStreamCache.writeObject(cache);
            objectOutputStreamCache.close();
            fileOutputStreamCache.close();
            System.out.println("Cached Sites written");

            FileOutputStream fileOutputStreamBlocked = new FileOutputStream(FileNames.BLOCKED_FILE.getFileName());
            ObjectOutputStream objectOutputStreamBlocked = new ObjectOutputStream(fileOutputStreamBlocked);
            objectOutputStreamBlocked.close();
            fileOutputStreamBlocked.close();
            System.out.println("Blocked sites list saved");

            try {
                // Close all servicing threads
                for (Thread thread : servicingThreads) {
                    if (thread.isAlive()) {
                        System.out.println(String.format("Waiting on thread with %d ID to close..", thread.getId()));
                        thread.join();
                        System.out.println("closed");
                    }
                }
            } catch (InterruptedException e) {
                System.out.println("InterruptedException occurred");
            }
        } catch (FileNotFoundException e) {
            System.out.println("FileNotFoundException occurred");
        } catch (IOException e) {
            System.out.println("IOException occurred");
        }
    }

    /**
     * Looks for File in cache
     *
     * @param url of requested file
     * @return File if file is cached, null otherwise
     */
    public static File getCachedPage(String url) {
        return cache.get(url);
    }

    /**
     * Adds a new page to the cache
     *
     * @param url         URL of webpage to cache
     * @param fileToCache File Object pointing to File put in cache
     */
    public static void addCachedPage(String url, File fileToCache) {
        cache.put(url, fileToCache);
    }

    /**
     * Check if a URL is blocked by the proxy
     *
     * @param url URL to check
     * @return true if URL is blocked, false otherwise
     */
    public static boolean isBlocked(String url) {
        if (blockedSites.get(url) != null) {
            return true;
        } else {
            return false;
        }
    }

    /**
     * Creates a management interface which can dynamically update the proxy configurations blocked Lists currently blocked sites
     * cached Lists currently cached sites close Closes proxy server Adds to list of blocked sites
     */
    @Override
    public void run() {
        Scanner in = new Scanner(System.in);
        String command;

        while (running) {
            System.out.println("Enter new site to block, or type:\n"
                                   + "\"blocked\" to see blocked sites\n"
                                   + "\"cached\" to see cached sites\n"
                                   + "\"close\" to close server");

            command = in.nextLine();

            switch (command.toLowerCase()) {
                case "blocked":
                    System.out.println("Blocked sites:");
                    for (String key : blockedSites.keySet()) {
                        System.out.println(key);
                    }
                    System.out.println();
                    break;
                case "cached":
                    System.out.println("Cached sites:");
                    for (String key : cache.keySet()) {
                        System.out.println(key);
                    }
                    System.out.println();
                    break;
                case "close":
                    running = false;
                    closeServer();
                    break;
                default:
                    blockedSites.put(command, command);
                    System.out.println(String.format("\n%s blocked successfully\n", command));
                    break;
            }
        }
        in.close();
    }
}
