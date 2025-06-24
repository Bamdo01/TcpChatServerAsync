using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Unicode;
using System.Threading.Tasks;
using NetMQ;
using NetMQ.Sockets;
using NEXTCHATServ.Managers;
using NEXTCHATServ.Model;

namespace TcpChatServerAsync.Core
{
    // 클라이언트 연결을 처리하는 static 클래스
    public static class ClientHandler
    {
        //        // 클라이언트의 요청을 수신하고 응답을 처리하는 메서드
        //        public static void HandleClient(TcpClient client)
        //        {
        //            // 클라이언트와의 네트워크 스트림 생성
        //            NetworkStream stream = client.GetStream();
        //            byte[] buffer = new byte[1024]; // 수신 데이터 저장용 버퍼

        //            while (true)
        //            {
        //                try
        //                {
        //                    // 스트림으로부터 데이터를 읽음 (최대 1024바이트)
        //                    int bytesRead = stream.Read(buffer, 0, buffer.Length);

        //                    // 0 바이트 읽은 경우 → 연결 종료된 것임
        //                    if (bytesRead == 0) break;

        //                     //받은 데이터를 UTF8 문자열로 변환(디버깅용)
        //                    //string message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        //                    //Console.WriteLine($"수신: {message}");

        //                    //// 명령어 처리기(CommandProcessor)에 메시지를 전달하고 응답 받기
        //                    /// 애초에 반환값을 바이트로 만들기
        //                    byte[] response = CommandProcessor.ProcessCommand(buffer, client);

        //                    // 응답이 있다면 클라이언트에 전송
        //                    if (response != null && response.Length > 0)
        //                    {
        //                        stream.Write(response, 0, response.Length);
        //                    }

        //                }
        //                catch
        //                {
        //                    // 예외 발생 시 연결 종료 처리
        //                    ClientManager.TryRemoveClient(client);
        //                    break;
        //                }
        //            }

        //            // 클라이언트 소켓 닫기
        //            client.Close();
        //        }

        public static void HandleZmqMessage(NetMQSocket router, NetMQMessage request)
        {
            if (request.FrameCount < 3)
                return;

            var clientId = request[0]; // NetMQFrame
            var body = request[2].ConvertToString();

            Console.WriteLine($"[수신] {clientId.ConvertToString()} : {body}");

            // 실제 처리 로직
            byte[] input = Encoding.UTF8.GetBytes(body);
            byte[] response = CommandProcessor.ProcessCommand(input); // ← 여기도 clientId 전달

            if (response != null && response.Length > 0)
            {
                var responseText = Encoding.UTF8.GetString(response);
                //Console.WriteLine($"[응답] {clientId.ConvertToString()} : {responseText}");
                Console.WriteLine($"[응답] {response}");


                // 응답 전송
                NetMQMessage reply = new NetMQMessage();
                reply.Append(clientId);
                reply.AppendEmptyFrame();
                reply.Append(response);

                router.SendMultipartMessage(reply);
                //router.SendFrame(response);
            }
        }


        public static void HandleZmqChatMessage(string body)
        {
            Console.WriteLine($"[수신] {body}");

            byte[] input = Encoding.UTF8.GetBytes(body);
            CommandProcessor.ProcessCommand(input); // 응답은 무시
        }



    }
}
