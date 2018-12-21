using CoreDemo.interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.Services
{
    public class CloudMailService: IMailService
    {
        private readonly string _mailTo = "admin@qq.com";
        private readonly string _mailFrom = "noreply@alibaba.com";

        public void Send(string subject, string msg)
        {
            Debug.WriteLine($"从{_mailFrom}给{_mailTo}通过{nameof(CloudMailService)}发送了邮件");
        }
    }
}
