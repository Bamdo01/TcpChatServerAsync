using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NEXTCHATServ.Managers;
using NetMQ.Sockets;
using NetMQ;
using TcpChatServerAsync.Network;
using TcpChatServerAsync.Model;


namespace TcpChatServerAsync.Core
{
    public static class ChatBroadcaster
    {
        private static PublisherSocket _pub;

        public static void SetSocket(PublisherSocket socket)
        {
            _pub = socket;
        }

        public static void BroadcastChat(ChatMessage chatMsg, string senderId)
        {
            if (_pub == null)
            {
                Console.WriteLine("❌ PUB 소켓이 초기화되지 않았습니다.");
                return;
            }

            byte[] packet = PacketBuilder.BuildChatPacket(chatMsg);

            ////브로드캐스트
            _pub.SendFrame(packet);

            Console.WriteLine($"[브로드캐스트] {chatMsg.FromUserId}: {chatMsg.Content}");
        }
    }
}
