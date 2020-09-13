using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Utils;

namespace Sender
{
    class SenderSocket
    {
        private Socket _socket;

        public bool IsConnected;

        public SenderSocket()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect(string ipAddress, int port)
        {
            _socket.BeginConnect(new IPEndPoint(IPAddress.Parse(ipAddress), port), ConnectedCallback, null);
            Thread.Sleep(3000);
        }

        private void ConnectedCallback(IAsyncResult asyncResult)
        {
            if (_socket.Connected)
            {
                Console.WriteLine("Sender connected to Broker.");
            }
            else
            {
                Console.WriteLine("Error: Sender not connected to Broker.");
                Logger.Log("Error: Sender not connected to Broker.");
            }

            IsConnected = _socket.Connected;
        }

        public void Send(byte[] data)
        {
            try
            {
                _socket.Send(data);
            }
            catch (ArgumentNullException e)
            {
                Logger.Log("Could not send data: " + e.Message);
            }
             catch (SocketException  e)
            {
                Logger.Log("Could not send data: " + e.Message);
            }
            catch (ObjectDisposedException e)
            {
                Logger.Log("Could not send data: " + e.Message);
            }
        }
    }
}
