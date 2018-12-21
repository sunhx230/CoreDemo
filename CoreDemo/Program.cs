using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CoreDemo
{
    /// <summary>
    /// asp.net core application实际就是控制台程序
    /// 它是一个调用asp.net core 相关库的console application. 
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            //Build()完之后返回一个实现了IWebHost接口的实例(WebHostBuilder), 然后调用Run()就会运行Web程序, 并且阻止这个调用的线程, 直到程序关闭.
            //顺序: Main -> Startup.ConfigureServices -> Startup.Configure
            CreateWebHostBuilder(args).Build().Run();
        }

        /// <summary>
        /// web程序需要一个宿主,所以 BuildWebHost这个方法就创建了一个WebHostBuilder
        /// asp.net core 自带了两种http servers, 一个是WebListener, 它只能用于windows系统, 另一个是kestrel, 它是跨平台的.kestrel是默认的web server, 就是通过UseKestrel()这个方法来启用的.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
