using System.Net.Sockets;

namespace Utils
{
    public class ConnectionInfo
    {
        // MARK: - Properties
        public Socket Socket { get; set; }
        public string Address { get; set; }
        public string Subject { get; set; }

        public const int BUFF_SIZE = 1024;
        public byte[] Buffer { get; set; }

        // MARK: - Initializers
        public ConnectionInfo()
        {
            this.Buffer = new byte[BUFF_SIZE];
        }
    }
}
