using System;
using System.Collections.Generic;
using System.Text;
using TcpChatServerAsync.Model;
using TcpChatServerAsync.Model.Enums;

namespace TcpChatServerAsync.Network
{
    public class PacketBuilder
    {
        private List<byte> _packet;

        // 생성자: 내부 바이트 리스트 초기화
        public PacketBuilder()
        {
            _packet = new List<byte>();
        }

        // 1바이트 값을 추가
        public PacketBuilder AddByte(byte value)
        {
            _packet.Add(value);
            return this;
        }

        // 2바이트(short) 값을 추가
        public PacketBuilder AddShort(short value)
        {
            _packet.AddRange(BitConverter.GetBytes(value));
            return this;
        }

        // 문자열을 [2바이트 길이 + 문자열] 형식으로 추가 (UTF-8 인코딩)
        public PacketBuilder AddString(string text)
        {
            byte[] stringBytes = Encoding.UTF8.GetBytes(text);
            _packet.AddRange(BitConverter.GetBytes((short)stringBytes.Length));
            _packet.AddRange(stringBytes);
            return this;
        }

        // 완성된 패킷을 byte 배열로 반환
        public byte[] Build()
        {
            return _packet.ToArray();
        }


        public static byte[] BuildChatPacket(ChatMessage chatMsg)
        {
            return new PacketBuilder()
                .AddByte((byte)CommandCode.TOCHATMSG)
                .AddByte((byte)chatMsg.Timestamp.Month)
                .AddByte((byte)chatMsg.Timestamp.Day)
                .AddShort((short)chatMsg.Timestamp.Year)
                .AddString(chatMsg.FromUserId)
                .AddString(chatMsg.Content)
                .Build();
        }
    }
}
