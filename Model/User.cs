using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpChatServerAsync.Model.Enums;

namespace TcpChatServerAsync.Model
{
    public class User
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }

        public GenderCode Gender { get; set; }       // enum으로 성별 지정
        public DateTime BirthDate { get; set; }      // 생년월일
    }

}
