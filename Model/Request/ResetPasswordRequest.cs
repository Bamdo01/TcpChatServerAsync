using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServerAsync.Model.Request
{
    public class ResetPasswordRequest
    {
        public string UserId { get; set; }
        public string NewPassword { get; set; }

    }
}
