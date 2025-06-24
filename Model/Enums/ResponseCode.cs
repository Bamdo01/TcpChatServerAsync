using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcpChatServerAsync.Model.Enums
{
    public enum ResponseCode
    {
        LoginSuccess = 30,       // 로그인 완료
        LoginFail = 31,          // 로그인 실패
        RegisterSuccess = 32,    // 회원가입 완료
        IdAlreadyExists = 33,     // 이미 존재하는 ID
        UnknownCommand
    }

}