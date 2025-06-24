using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServerAsync.Model.Request
{
    public class LoginRequest
    {
        public string UserId { get; set; }
        public string Password { get; set; }

        //public bool IsValid()
        //{
        //    return !string.IsNullOrEmpty(UserId) && !string.IsNullOrEmpty(Password);
        //}
    }
}
