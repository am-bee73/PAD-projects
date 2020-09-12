using System;
using System.Net;
using System.Net.Sockets;
using Utils;

namespace Broker
{
    class BrockerSocket
    {
        // MARK: - Properties
        private const int CONNECTIONS_LIMIT = 10;
        private Socket _socket;

        // MARK: - Initializers
        public BrockerSocket()
        {
            // Setup socket config
            this._socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        // MARK: - Methods
        public void Start(string ip, int port)
        {
            // Bind socket address
            this._socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            // Set connections limit
            this._socket.Listen(CONNECTIONS_LIMIT);

            // Accept connections
            this.AcceptConnection();
        }

        private void AcceptConnection()
        {
            this._socket.BeginAccept(this.AcceptedCallback, null);
        }

        private void AcceptedCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connectionInfo = new ConnectionInfo();

            try
            {
                // Setup connection info
                connectionInfo.Socket = this._socket.EndAccept(asyncResult);
                connectionInfo.Address = connectionInfo.Socket.RemoteEndPoint.ToString();

                // Start receiving
                connectionInfo.Socket.BeginReceive(connectionInfo.Buffer, 0, connectionInfo.Buffer.Length, SocketFlags.None, this.ReceiveCallBack, connectionInfo);
            }
            catch(Exception e)
            {
                Logger.Log("Can't accept: " + e.Message);
            }
            finally
            {
                // Accept another conenction
                this.AcceptConnection();
            }
        }

        private void ReceiveCallBack(IAsyncResult asyncResult)
        {
            ConnectionInfo connectionInfo = asyncResult.AsyncState as ConnectionInfo;

            try
            {
                // Read data from socket
                Socket senderSocket = connectionInfo.Socket;
                SocketError resp;
                int buffSize = senderSocket.EndReceive(asyncResult, out resp);

                if(resp == SocketError.Success)
                {
                    // We have data
                    byte[] payload = new byte[buffSize];
                    Array.Copy(connectionInfo.Buffer, payload,buffSize);

                    // Handle payload
                    DataHandler.Handle(payload, connectionInfo);
                } 
            }
            catch(Exception e)
            {
                Logger.Log("Can't receive data: " + e.Message);
            }
            finally
            {
                try
                {
                    // Start receiving new data
                    connectionInfo.Socket.BeginReceive(connectionInfo.Buffer, 0, connectionInfo.Buffer.Length, SocketFlags.None, this.ReceiveCallBack, connectionInfo);
                }
                catch (SocketException e)
                {
                    // Socket is disconnected
                    Console.WriteLine($"{e.Message}");
                    //Logger.Log("Can't accept: " + e.Message);

                    // Remove socket from storage
                    var address = connectionInfo.Socket.RemoteEndPoint.ToString();

                    // Close connection
                    connectionInfo.Socket.Close();
                }
                catch (Exception e)
                {
                    // Socket is disconnected
                    Logger.Log("Can't accept: " + e.Message);

                    // Get connection address
                    var address = connectionInfo.Socket.RemoteEndPoint.ToString();

                    // Remove socket from storage
                    ConnectionsStorage.RemoveConnection(address);

                    // Close connection
                    connectionInfo.Socket.Close();
                }
            }
        }
    }
}
