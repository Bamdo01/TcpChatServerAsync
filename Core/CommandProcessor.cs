using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using NetMQ;
using TcpChatServerAsync.Managers;
using TcpChatServerAsync.Model;
using TcpChatServerAsync.Database;
using TcpChatServerAsync.Network;
using TcpChatServerAsync.Model.Enums;

namespace TcpChatServerAsync.Core
{
    /// <summary>
    /// 클라이언트로부터 수신된 명령을 처리하는 클래스
    /// </summary>
    public static class CommandProcessor
    {
        /// <summary>
        /// 수신된 바이트 데이터를 해석하고 명령에 따라 적절히 처리
        /// </summary>
        public static byte[] ProcessCommand(byte[] buffer)
        {
            // 버퍼 파싱 객체 생성
            PacketParser parser = new PacketParser(buffer);

            // 명령 코드 읽기 (첫 번째 바이트)
            byte commandCode = parser.ReadByte();

            // 명령어 enum으로 캐스팅
            CommandCode command = (CommandCode)commandCode;

            // 명령 코드에 따라 분기 처리
            switch (command)
            {
                case CommandCode.REGISTER:
                    // 회원가입 처리
                    return HandleRegister(parser);

                case CommandCode.LOGIN:
                    // 로그인 처리
                    return HandleLogin(parser);

                //case CommandCode.FINDID:
                //    // ID 찾기 처리
                //    return HandleIdSearch(parser);

                case CommandCode.CHATMSG:
                    // 채팅 처리
                    return HandleChatMessage(parser);

                default:
                    // 알 수 없는 명령 처리
                    return new byte[] { (byte)ResponseCode.UnknownCommand };
            }
        }

        
        private static byte[] HandleRegister(PacketParser parser)
        {
            byte genderCodeByte = parser.ReadByte();      // 성별 (1 byte)
            byte birthMonth = parser.ReadByte(); // 예시: 5월
            byte birthDay = parser.ReadByte();  // 예시: 26일
            short birthYear = parser.ReadInt16(); // 예시: 1995년
            string userId = parser.ReadString();          // ID
            string password = parser.ReadString();        // Password                    
            string userPhone = parser.ReadString();       // 전화번호 (11자리 문자열)

            //바이트 타입인 genderCodeByte을 GenderCode로 형변환
            GenderCode gender = (GenderCode)genderCodeByte;

            //벌스 데이트에 저장
            DateTime birthDate = new(birthYear, birthMonth, birthDay);


            Console.WriteLine($"[회원가입 요청] ID: {userId}, PW: {password}, PN: {userPhone} 성별: {gender} 생일: {birthDate:yyyy-MM-dd}");

            // TODO: 실제 회원가입 처리 구현 필요
            bool a = UserRepository.IsUserIdExists(userId);

            if (!a)
            {
                //회원가입 로직
                User user = new User
                {
                    UserId = userId,
                    Password = password,
                    Phone = userPhone,
                    Gender = gender,
                   BirthDate = birthDate,
                };

                bool isInserted = UserRepository.InsertUser(user);

                if (!isInserted)
                {
                    
                    Console.WriteLine("회원가입 실패 데이터 포멧이 잘못되지 않을까.. 예외처");
                    return new byte[] { (byte)ResponseCode.IdAlreadyExists };
                }
                else
                {
                    Console.WriteLine("회원가입 성공");
                    return new byte[] { (byte)ResponseCode.RegisterSuccess };
                }


            }
            //실패 반환(중복아이디)
            Console.WriteLine("회원가입 실패(중복아이디)");
            return new byte[] { (byte)ResponseCode.IdAlreadyExists};
            

        }

        //로그인 요청 처리
        private static byte[] HandleLogin(PacketParser parser)
        {
            string userId = parser.ReadString();       // ID
            string password = parser.ReadString();     // PW
            

            Console.WriteLine($"[로그인 요청] ID: {userId}, PW: {password}");

            // 로그인 검증
            bool isLoginSuccess = UserRepository.Login(userId, password);

            if (isLoginSuccess)
            {
                // 클라이언트 목록에 추가
                ClientManager.TryAddClient(userId);

                Console.WriteLine($"[로그인 성공] 사용자: {userId}");

                //클라이언트 딕셔너리에 추가
                ClientManager.TryAddClient(userId);

                return new byte[] { (byte)ResponseCode.LoginSuccess }; // 0 또는 정의된 성공 코드
            }
            else
            {
                Console.WriteLine($"[로그인 실패] 사용자: {userId}");
                return new byte[] { (byte)ResponseCode.LoginFail }; // 실패 코드 정의 필요
            }
        }

        
        ////아이디 검색 처리
        //private static byte[] HandleIdSearch(PacketParser parser)
        //{
        //    string userId = parser.ReadString();       // ID

        //    Console.WriteLine($"[ID 검색 요청] ID: {userId}");

        //    // TODO: ID 존재 여부 확인 로직 추가 필요
        //    return new byte[] { (byte)ResponseCode.Success }; // 또는 "IdNotFound"
        //}


        //나중에 시간 추가하기
        //채팅 요청 처리
        private static byte[] HandleChatMessage(PacketParser parser)
        {
            string userId = parser.ReadString();    // 보낸 사람 ID
            string message = parser.ReadString();   // 채팅 내용

            Console.WriteLine($"아이디: {userId} 메세지:{message}");

            ChatMessage chatMsg = new ChatMessage
            {
                FromUserId = userId,
                Content = message,
                Timestamp = DateTime.Now
            };
            
            UserRepository.SaveChatMessage(chatMsg);

            ChatBroadcaster.BroadcastChat(chatMsg, userId);

            return null; // 단순히 수신 성공 응답 해줄 필요 없는데
        }
    }
}
