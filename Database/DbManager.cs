using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace TcpChatServerAsync.Database
{
    public static class DbManager
    {
        private static string connStr = "Server=localhost;Database=NEXTCHAT;User ID=root;Password=1234;";

        public static MySqlConnection GetConnection()
        {
            return new MySqlConnection(connStr);
        }

        // DB 초기화 함수 - 서버 시작 시 호출
        public static void InitializeDatabase()
        {
            Console.WriteLine("DadaBase Initialize...");
            try
            {
                using (MySqlConnection conn = GetConnection())
                {
                    conn.Open();

                    // users 테이블 생성 (없으면 자동 생성)
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS users (
                    id INT PRIMARY KEY AUTO_INCREMENT,
                    userid VARCHAR(50) NOT NULL UNIQUE,
                    password VARCHAR(100) NOT NULL,
                    salt VARCHAR(100) NOT NULL,
                    phone VARCHAR(20),
                    gender VARCHAR(10) NOT NULL,
                    birthdate DATE NOT NULL
                        );";

                    string createChatTableQuery = @"
                        CREATE TABLE IF NOT EXISTS chat_messages (
                        id INT PRIMARY KEY AUTO_INCREMENT,
                        sender_id VARCHAR(50) NOT NULL,
                        message TEXT NOT NULL,
                        sent_at DATETIME DEFAULT CURRENT_TIMESTAMP
                        );";

                    using var cmd1 = new MySqlCommand(createTableQuery, conn);
                    cmd1.ExecuteNonQuery();

                    using var cmd2 = new MySqlCommand(createChatTableQuery, conn);
                    cmd2.ExecuteNonQuery();

                    Console.WriteLine("[DB 초기화] users 테이블 확인 완료 ");

                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine("[DB 초기화 오류] " + ex.Message);
                Environment.Exit(1); // DB 연결 실패 시 서버 종료
            }
        }
    }

}
