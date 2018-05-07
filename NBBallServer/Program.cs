using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NBBallServer
{
    class Program
    {
        static void Main(string[] args)
        {
            int hostPort;
            Console.Write("Input listening port number in integer : ");
            hostPort = int.Parse(Console.ReadLine());
            NetworkHandler_server serverHandler = 
                new NetworkHandler_server(hostPort);
            Console.WriteLine("Listening on {0}", hostPort);
            if(serverHandler.Listen() == 0)
                Console.WriteLine("Connected!");
            while (true)
            {
                serverHandler.Write(Console.ReadLine());
                Console.WriteLine(serverHandler.Read());
            }
        }
    }
    class NetworkHandler_server
    {
        private IPAddress clientIP;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private int listenPort;
        private NetworkStream networkStream;
        private byte[] r_buffer = new byte[1024];
        private byte[] w_buffer = new byte[1024];
        public NetworkHandler_server(int port)
        {
            tcpListener = new TcpListener(IPAddress.Any, port);
            tcpListener.Start();
        }
        public int Listen()
        {
            try
            {
                tcpClient = tcpListener.AcceptTcpClient();
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
            int len = networkStream.Read(r_buffer, 0, r_buffer.Length);
            return Encoding.Default.GetString(r_buffer).Trim('\0');
        }
        public int Write(string writeContent)
        {
            try
            {
                w_buffer = Encoding.UTF8.GetBytes(writeContent);
                networkStream.Write(w_buffer, 0, w_buffer.Length);
            }
            catch (Exception e)
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
