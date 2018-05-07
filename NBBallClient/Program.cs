using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
namespace NBBallClient
{
    class Program
    {
        static void Main(string[] args)
        {
            string hostIP;
            int hostPort;
            Console.Write("Input host's IP in string : ");
            hostIP = Console.ReadLine();
            Console.Write("Input host's Port in integer : ");
            hostPort = int.Parse(Console.ReadLine());
            NetworkHandler_client clientHandler = 
                new NetworkHandler_client(hostIP, hostPort);
            Console.WriteLine("connecting {0}:{1}", hostIP, hostPort);
            if(clientHandler.Connect() == 0)
                Console.WriteLine("connected!");
            while (true)
            {
                clientHandler.Write(Console.ReadLine());
                Console.WriteLine(clientHandler.Read());
            }
        }
    }
    class NetworkHandler_client
    {
        private IPAddress hostIP;
        private TcpClient tcpClient = new TcpClient();
        private int hostPort;
        private NetworkStream networkStream;
        private byte[] r_buffer = new byte[1024];
        private byte[] w_buffer = new byte[1024];
        public NetworkHandler_client(string ip, int port)
        {
            hostIP = IPAddress.Parse(ip);
            hostPort = port;
        }
        public int Connect()
        {
            try
            {
                tcpClient.Connect(hostIP, hostPort);
                networkStream = tcpClient.GetStream();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                return 1;
            }
            return 0;
        }
        public string Read()
        {
            networkStream.Read(r_buffer, 0, r_buffer.Length);
            return Encoding.Default.GetString(r_buffer).Trim('\0');
        }
        public int Write(string writeContent)
        {
            try
            {
                w_buffer = Encoding.UTF8.GetBytes(writeContent);
                networkStream.Write(w_buffer, 0, w_buffer.Length);
            }
            catch(Exception e)
            {
                return 1;
            }
            return 0;
        }
        public void Close()
        {
            networkStream.Close();
            tcpClient.Close();
        }

    }
}
