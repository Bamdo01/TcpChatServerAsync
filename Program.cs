using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TcpChatServerAsync.Core;
using TcpChatServerAsync.Database;
using TcpChatServerAsync.Managers;


namespace TcpChatServerAsync
{
    class Program
    {
        static void Main(string[] args)
        {
            //간단한 명령어 모음
            StartAdminCommandListener();

            //데이터베이스 이니셜라이징
            DbManager.InitializeDatabase();

            // 서버 시작 (9000번 포트에서 대기)
            Server.Start();

            


        }

        public static void StartAdminCommandListener()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    string? command = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(command))
                        continue;

                    switch (command.Trim().ToLower())
                    {
                        case "online":
                            var users = ClientManager.GetAllUsernames();
                            Console.WriteLine("\n[현재 접속 중인 사용자 목록]");
                            if (users.Count == 0)
                            {
                                Console.WriteLine(" - 없음");
                            }
                            else
                            {
                                foreach (var user in users)
                                {
                                    Console.WriteLine($" - {user}");
                                }
                            }
                            Console.WriteLine();
                            break;

                        case "exit":
                            Console.WriteLine("서버 종료 중...");
                            Environment.Exit(0);
                            break;

                        default:
                            Console.WriteLine("[사용 가능한 명령어] online, exit");
                            break;
                    }
                }
            });
        }


    }
}   