﻿using System;
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

            //이 앞은 게임 부분
            GameController gameController = new GameController();
            while (true)
            {
                gameController.Initialize();
                while (true)
                {
                    string query = gameController.GetQueryNumber();
                    serverHandler.Write(query);
                    serverHandler.Write(gameController.EvaluateNumber(serverHandler.Read()));
                    gameController.DisplayResult(serverHandler.Read());
                    if (gameController.ChkPlayerWon())
                    {
                        serverHandler.Write("OpponentWon");
                        serverHandler.Read();
                        gameController.EndSequence(winner: true);
                        break;
                    }
                    else
                    {
                        serverHandler.Write("Continue");
                        if (serverHandler.Read() == "OpponentWon")
                        {
                            gameController.EndSequence(winner: false);
                            
                        }
                    }
                }

            }
        }
    }
    class NetworkHandler_server
    {
        /* 네트워크 핸들링 클래스
         * 통신과 관련된 기능들은 전부 여기서 구현한다
         * 필드 모두 private 선언
         */
        private IPAddress clientIP;
        private TcpListener tcpListener;
        private TcpClient tcpClient;
        private int listenPort;
        private NetworkStream networkStream;
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
            byte[] r_buffer = new byte[1024];
            networkStream.Read(r_buffer, 0, r_buffer.Length);
            return Encoding.UTF8.GetString(r_buffer).Trim('\0'); //널 문자 트림 후 리턴
        }
        public int Write(string writeContent)
        {
            byte[] w_buffer = new byte[1024];
            try
            {
                w_buffer = Encoding.UTF8.GetBytes(writeContent); // UTF8 인코딩
                networkStream.Write(w_buffer, 0, w_buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
    class GameController
    {
        private string mySecretNumber;
        private bool isGaming;
        private int score;
        private int strikeCount;

        public void Initialize()
        {
            Console.Write("당신의 비밀번호를 입력하세요 : ");
            mySecretNumber = Console.ReadLine();
            while (mySecretNumber.Length != 4 || int.TryParse(mySecretNumber, out int a) == false)
            {
                Console.Write("4자리 정수로 입력해 주세요 : ");
                mySecretNumber = Console.ReadLine();
            }
            Console.WriteLine("숫자가 입력되었습니다. 게임을 시작합니다!");
            isGaming = true;
        }
        public string GetQueryNumber()
        {
            Console.Write("질문할 숫자를 입력하세요 : ");
            string input;
            input = Console.ReadLine();
            while (input.Length != 4 || int.TryParse(input, out int a) == false)
            {
                Console.Write("4자리 정수로 입력해 주세요 : ");
                input = Console.ReadLine();
            }
            return input;
        }
        public string EvaluateNumber(string query)
        {
            strikeCount = 0;
            int ballCount = 0;
            for (int i = 0; i < 4; i++)
            {
                if (query[i] == mySecretNumber[i])
                {
                    strikeCount++;
                }
                else if (mySecretNumber.Contains(query[i].ToString()))
                {
                    ballCount++;
                }
            }
            return strikeCount + " " + ballCount;
        }
        public void DisplayResult(string result)
        {
            string[] parsedResult = result.Split(' ');

            Console.WriteLine(parsedResult[0]+"스트라이크 "+parsedResult[1]+"볼");
            strikeCount = int.Parse(parsedResult[0]);
        }


        public bool ChkPlayerWon()
        {
            if (strikeCount == 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void EndSequence(bool winner)
        {
            if (winner)
            {
                Console.WriteLine("상대의 숫자를 맞추셨습니다! 축하합니다!");
            }
            else
            {
                Console.WriteLine("상대가 먼저 숫자를 맞추었습니다...");
            }
        }
    }
}
