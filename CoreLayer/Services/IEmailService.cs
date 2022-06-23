using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLayer.Services
{
    public interface IEmailService
    {
        Task SendEmail(string To, string Title, string Body);
    }
}
