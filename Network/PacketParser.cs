using System;
using System.Text;

namespace TcpChatServerAsync.Network
{
    /// <summary>
    /// 네트워크로 수신된 byte 배열을 구조화된 데이터(명령 코드, 문자열 등)로 파싱하는 클래스
    /// </summary>
    public class PacketParser
    {
        private readonly byte[] buffer; // 원본 버퍼
        private int index;              // 현재 읽기 위치 인덱스

        // 생성자: 파싱할 데이터를 전달받음
        public PacketParser(byte[] data)
        {
            buffer = data;
            index = 0;
        }

        /// <summary>
        /// 1바이트 읽기 (예: 명령 코드)
        /// </summary>
        public byte ReadByte()
        {
            //버퍼의 0번쨰 인덱스를 반환후 인덱스 더하기
            return buffer[index++];
        }

        /// <summary>
        /// 2바이트를 short(Int16) 값으로 읽기
        /// </summary>
        public short ReadInt16()
        {
            //16비트 즉 2바이트 읽기
            short value = BitConverter.ToInt16(buffer, index);
            index += 2;
            return value;
        }

        public int ReadInt32()
        {
            //16비트 즉 4바이트 읽기
            int value = BitConverter.ToInt32(buffer, index);
            index += 4;
            return value;

        }

        /// <summary>
        /// [2바이트 길이][문자열 바이트들] 형식으로 인코딩된 UTF-8 문자열을 읽기
        /// </summary>
        public string ReadString()
        {
            short length = ReadInt16(); // 문자열 길이 먼저 읽기
            string result = Encoding.UTF8.GetString(buffer, index, length); // 문자열 바이트 디코딩
            index += length;
            return result;
        }

        /// <summary>
        /// 현재 읽기 인덱스 (디버깅용)
        /// </summary>
        public int CurrentIndex => index;
    }
}