using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServerAsync.Model
{
    public class ChatMessage
    {
        public string FromUserId { get; set; }
        public string Content { get; set; }

        // 메시지 전송 시간
        public DateTime Timestamp { get; set; }
    }
}
