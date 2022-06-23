using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api_Center.Models
{
    public interface IMethodHelper
    {
        string CreatToken(int UserId, string UserName);
        string CreatRefreshToken();
    }
}
