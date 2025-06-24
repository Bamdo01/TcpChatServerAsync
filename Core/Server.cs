
using NetMQ.Sockets;
using NetMQ;
using System.ServiceModel.Channels;
using System.Text;

namespace TcpChatServerAsync.Core
{
    public static class Server
    {
        public static void Start()
        {
            using var router = new RouterSocket();
            using var pull = new PullSocket();
            using var pub = new PublisherSocket();

            router.Bind("tcp://*:5555");  // ROUTER 소켓 포트
            pull.Bind("tcp://*:5556");    // PULL 소켓 포트
            pub.Bind("tcp://*:5557");     // PUB 소켓 포트

            ChatBroadcaster.SetSocket(pub);

            Console.WriteLine("[서버] ROUTER:5555 / PULL:5556 / PUB:5557 열림");

            // Poller를 사용해 두개의 소켓을 동시에 감시
            NetMQPoller poller = new NetMQPoller { router, pull };

            // 이벤트 핸들러 등록
            router.ReceiveReady += OnRouterReceiveReady;
            pull.ReceiveReady += OnPullReceiveReady;


            // 이벤트 루프 시작 (비동기 수신 대기)
            poller.Run();
        }

        private static void OnRouterReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage request = null;


            while (e.Socket.TryReceiveMultipartMessage(ref request))
            {
                // 수신한 메시지를 비동기 스레드에서 처리
                NetMQSocket router = e.Socket; // 이게 ROUTER 소켓 그 자체

                ThreadPool.QueueUserWorkItem(_ =>
                {
                    try
                    {
                        ClientHandler.HandleZmqMessage(router, request);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[예외 발생] {ex}");
                    }

                });

            }
        }
        private static void OnPullReceiveReady(object sender, NetMQSocketEventArgs e)
        {
            byte[] msgBytes;

            while (e.Socket.TryReceiveFrameBytes(out msgBytes))
            {
                // 바이트 데이터를 문자열로 변환 (UTF-8 기준)
                string msg = Encoding.UTF8.GetString(msgBytes);

                // 콘솔 출력
                Console.WriteLine($"이것이 리얼이다 수신 @ {DateTime.Now:HH:mm:ss}] {msg}");

                // 비동기로 메시지 처리
                ThreadPool.QueueUserWorkItem(_ => ClientHandler.HandleZmqChatMessage(msg));
            }
        }

    }

}




