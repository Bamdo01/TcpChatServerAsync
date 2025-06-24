using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using TcpChatServerAsync.Model;

namespace TcpChatServerAsync.Database
{
    public static class UserRepository
    {
        public static bool InsertUser(User user)
        {
            using (MySqlConnection conn = DbManager.GetConnection())
            {
                conn.Open();

                // [1] 서버에서 salt 생성
                string salt = SecurityHelper.GenerateSalt();

                // [2] 클라이언트에서 받은 비밀번호 해시와 salt를 조합해 최종 해시
                string finalHash = SecurityHelper.HashWithSalt(user.Password, salt);

                // [3] salt 컬럼 추가된 쿼리
                string query = @"INSERT INTO users (userid, password, salt, phone, gender, birthdate)
                         VALUES (@id, @pw, @salt, @pn, @gender, @birth)";

                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", user.UserId);
                cmd.Parameters.AddWithValue("@pw", finalHash);     // 서버 해싱 후 저장
                cmd.Parameters.AddWithValue("@salt", salt);        // salt도 함께 저장
                cmd.Parameters.AddWithValue("@pn", user.Phone);
                cmd.Parameters.AddWithValue("@gender", user.Gender.ToString());
                cmd.Parameters.AddWithValue("@birth", user.BirthDate.ToString("yyyy-MM-dd"));

                return cmd.ExecuteNonQuery() > 0;
            }
        }

        //참이면 중복아이디 있는겁니당
        public static bool IsUserIdExists(string userId)
        {
            using (MySqlConnection conn = DbManager.GetConnection())
            {
                conn.Open();

                string query = "SELECT COUNT(*) FROM users WHERE userid = @id";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@id", userId);

                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
        }

        //로그인 함수
        public static bool Login(string userid, string passwordHashFromClient)
        {
            try
            {
                using (MySqlConnection conn = DbManager.GetConnection())
                {
                    conn.Open();

                    // 1. password와 salt 모두 불러옴
                    string query = @"
                SELECT password, salt 
                FROM users 
                WHERE userid = @userid;
            ";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userid", userid);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string storedPasswordHash = reader.GetString("password");
                                string storedSalt = reader.GetString("salt");

                                // 2. 클라이언트 해시 + salt 조합 → 최종 해시 계산
                                string combinedHash = SecurityHelper.HashWithSalt(passwordHashFromClient, storedSalt);

                                // 3. 해시 비교
                                return combinedHash == storedPasswordHash;
                            }
                            else
                            {
                                return false; // 해당 ID 없음
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[로그인 오류] {ex.Message}");
                return false;
            }
        }



        //DB에 채팅 로그 저장하는 함수
        public static bool SaveChatMessage(ChatMessage chatMessage)
        {
            using (MySqlConnection conn = DbManager.GetConnection())
            {
                conn.Open();

                string query = @"INSERT INTO chat_messages (sender_id, message, sent_at)
                         VALUES (@sender, @msg, @time)";

                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@sender", chatMessage.FromUserId);
                    cmd.Parameters.AddWithValue("@msg", chatMessage.Content);
                    cmd.Parameters.AddWithValue("@time", chatMessage.Timestamp);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        //로그인





    }
}
