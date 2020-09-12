using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils;

namespace Receiver
{
    class ReceiverSocket
    {
        private Socket _socket;
        private string _topic;
    

        public ReceiverSocket(string topic)
        {
            _topic = topic;
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }


        public void Connect(string ipAddress, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectCallback, null);

            Console.WriteLine("Waiting for a connection");

        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            if (_socket.Connected)
            {
                Console.WriteLine("Receiver connected to broker. ");
                Receiver();
                StartReceive();
            }

            else
            {
                Console.WriteLine("Error: Receiver could not connect to broker.");
            }
        }

        private void Receiver()
        {
            var data = Encoding.UTF8.GetBytes("receive#" + _topic);
            Send(data);
        }

        private void StartReceive()
        {

            ConnectionInfo connection = new ConnectionInfo();
            connection.Socket = _socket;

            _socket.BeginReceive(connection.Buffer, 0, connection.Buffer.Length, 
                SocketFlags.None, ReceiveCallback, connection);
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            ConnectionInfo connectionInfo = asyncResult.AsyncState as ConnectionInfo;
            try
            {
                SocketError response;
                int buffsize = _socket.EndReceive(asyncResult, out response);
                if (response == SocketError.Success)
                {
                    byte[] payloadBytes = new byte[buffsize];
                    Array.Copy(connectionInfo.Buffer,payloadBytes, payloadBytes.Length);

                    PayloadHandler.Handle(payloadBytes);

                }
           }
            catch(Exception e)
            {
                Console.WriteLine($"Can't receive data from broker. {e.Message}");

            }
            finally
            {
                try
                {
                    connectionInfo.Socket.BeginReceive(connectionInfo.Buffer, 0, connectionInfo.Buffer.Length,
                        SocketFlags.None, ReceiveCallback, connectionInfo);
                }
                catch(Exception e)
                {
                    Console.WriteLine($"{e.Message}");
                    connectionInfo.Socket.Close();

                }
            }
        }

        private void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);

            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not send data: {e.Message}");
            }
        }

    }
}
