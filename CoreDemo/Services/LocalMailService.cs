using CoreDemo.interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.Services
{
    public class LocalMailService:IMailService
    {
        private readonly string _mailTo = Startup.Configuration["mailSettings:mailToAddress"];
        private readonly string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];

        private readonly ILogger<LocalMailService> _logger;

        public LocalMailService(ILogger<LocalMailService> logger)
        {
            _logger = logger;
        }

        public void Send(string subject, string msg)
        {
            LocalMailService localMailService = this;
            _logger.LogInformation($"从{_mailFrom}给{_mailTo}通过{nameof(LocalMailService)}发送了邮件");
        }
    }
}
