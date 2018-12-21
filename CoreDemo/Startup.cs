using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Entities;
using CoreDemo.interfaces;
using CoreDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using NLog.Extensions.Logging;

namespace CoreDemo
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940


        /*
         * 通过环境变量控制配置文件
         * asp.net core 支持各式各样的配置方法，包括使用JSON，xml， ini文件，环境变量，命令行参数等等。建议使用的还是JSON。
         * 我们的相关配置信息是动态可配置的,就在网站的根目录下创建一个appSettings.json的文件
         * appSettings.json里面的值就需要使用实现了IConfiguration这个接口的对象。建议的做法是：在Startup.cs里面注入IConfiguration（这个时候通过CreateDefaultBuilder方法，它已经建立好了），然后把它赋给一个静态的property：*/
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// 用来把services(各种服务, 例如identity, ef, mvc等等包括第三方的, 或者自己写的)加入(register)到container(asp.net core的容器)中去, 
        /// 并配置这些services. 这个container是用来进行dependency injection的(依赖注入). 所有注入的services(此外还包括一些框架已经注册好的services) 在以后写代码的时候, 
        /// 都可以将它们注入(inject)进去. 例如下面的Configure方法的参数, app, env, loggerFactory都是注入进去的services
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddMvc() //注册mvc中间件到Core的容器中
                .AddJsonOptions(options => ////asp.net core 2.0 默认返回的结果格式是Json, 并使用json.net对结果默认做了camel case的转化(大概可理解为首字母小写). 
                {
                    if (options.SerializerSettings.ContractResolver is DefaultContractResolver resolver)
                        resolver.NamingStrategy = null;  //设置为null之后。首字母不做camelcase转化
                })
                .AddMvcOptions(options =>
                {
                    /*
                     * 内容协商 Content Negotiation
                     * 如果 web api提供了多种内容格式, 那么可以通过Accept Header来选择最好的内容返回格式: 例如:
                        application/json, application/xml等等
                        如果设定的格式在web api里面没有, 那么web api就会使用默认的格式.
                        asp.net core 默认提供的是json格式, 也可以配置xml等格式.
                     */
                    options.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());   //向输出格式的集合中添加xml格式
                });
            /*
             * 我们要把这个LocalMailService注册给Container。打开Startup.cs进入ConfigureServices方法。这里面有三种方法可以注册service：AddTransient，AddScoped和AddSingleton，这些都表示service的生命周期。
                transient的services是每次请求（不是指Http request）都会创建一个新的实例，它比较适合轻量级的无状态的（Stateless）的service。
                scoped的services是每次http请求会创建一个实例。
                singleton的在第一次请求的时候就会创建一个实例，以后也只有这一个实例，或者在ConfigureServices这段代码运行的时候创建唯一一个实例。
             */
            //当需要IMailService的一个实现的时候，Container就会提供一个指定继承IMailService接口的实例。
            services.AddTransient<IMailService, LocalMailService>(); //为了符合Di也就是依赖注入的原则新增了一个接口IMailService




            //先在Container中注册MyContext上下文，然后就可以在依赖注入中使用了。使用AddDbContext这个Extension method为MyContext在Container中进行注册，它默认的生命周期使Scoped(每次http都会创建一个实例)
            //并且同时为其设置数据库连接字符串
            services.AddDbContext<MyContext>(options => options.UseSqlServer(Configuration["connectionStrings:DbConnectionString"]));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// asp.net core程序用来具体指定如何处理每个http请求的, 例如我们可以让这个程序知道我使用mvc来处理http请求, 那就调用app.UseMvc()这个方法就行. 
        /// </summary>
        /// <param name="app">提供配置应用程序请求管道的机制</param>
        /// <param name="env">保存了asp.net core程序的基本环境信息</param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            /*
             * 请求管道: 那些处理http requests并返回responses的代码就组成了request pipeline(请求管道).
             * 中间件: 我们可以做的就是使用一些程序来配置那些请求管道 request pipeline以便处理requests和responses. 比如处理验证(authentication)的程序, 连MVC本身就是个中间件(middleware).
             * 每层中间件接到请求后都可以直接返回或者调用下一个中间件. 一个比较好的例子就是: 在第一层调用authentication验证中间件, 如果验证失败, 那么直接返回一个表示请求未授权的response.
             * 
             * Request->middleware(需要用到的中间件)处理->Response   形成了Request pipeline
             */
            //loggerFactory.AddProvider(new NLogLoggerProvider()); //将Nlog注入到日志的工厂中
            loggerFactory.AddNLog(); //该方法是NLog为工厂写的扩展方法方法同上

            if (env.IsDevelopment())//这个middleware只会在Development环境下被调用.相关配置可以在项目属性中的Debug页面中看见
            {
                app.UseDeveloperExceptionPage(); //就是一个middleware, 当exception发生的时候, 这段程序就会处理它. 
            }

            app.UseMvc();

            //app.Run(async (context) =>   
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});
        }
    }
}
