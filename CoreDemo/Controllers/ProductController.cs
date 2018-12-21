using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreDemo.Dtos;
using CoreDemo.Entities;
using CoreDemo.interfaces;
using CoreDemo.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreDemo.Controllers
{
    /*
     * 这里不配置Routing 路由请求是找不到相关处理的控制和action
     *  路由有两种Convention-based (按约定), attribute-based(基于路由属性配置的). 
     *  其中convention-based (基于约定的) 主要用于MVC (返回View或者Razor Page那种的).
     *  Web api 推荐使用attribute-based.
     *  这种基于属性配置的路由可以配置Controller或者Action级别, uri会根据Http method然后被匹配到一个controller里具体的action上.
     *  常用的Http Method有:
                Get, 查询, Attribute: HttpGet, 例如: '/api/product', '/api/product/1'
                POST, 创建, HttpPost, '/api/product'
                PUT 整体修改更新 HttpPut, '/api/product/1'
                PATCH 部分更新, HttpPatch, '/api/product/1'
                DELETE 删除, HttpDelete, '/api/product/1
     *  还有一个Route属性(attribute)也可以用于Controller层, 它可以控制action级的URI前缀.
     */
    [Route("api/product")] //使用[Route("api/[controller]")], 它使得整个Controller下面所有action的uri前缀变成了"/api/product", 其中[controller]表示XxxController.cs中的Xxx(其实是小写).也可以具体指定, [Route("api/product")], 这样做的好处是, 如果ProductController重构以后改名了, 只要不改Route里面的内容, 那么请求的地址不会发生变化.
    public class ProductController: Controller
    {
        private readonly ILogger<ProductController> _logger;
        private readonly IMailService _mailService;

        private readonly MyContext _context;

        public ProductController(ILogger<ProductController> logger, IMailService mailService, MyContext context)
        {
            _logger = logger;

            /*
             * 此处,不需要知道具体用那个服务去处理,运行时容器会将对应IMailService接口的实例注入进来
             * 前提是在Startup->ConfigureServices方法中注册过
             */
            _mailService = mailService;

            _context = context;
        }
        //然后在GetProducts方法上面, 写上HttpGet, 也可以写HttpGet(). 它里面还可以加参数,例如: HttpGet("all"), 那么这个Action的请求的地址就变成了 "/api/product/All".
        [HttpGet]
        public JsonResult GetProducts()
        {
            return new JsonResult(ProductService.Current.Products);
        }

        [HttpGet("{id}")]
        public JsonResult GetProduct(int id)
        {
            var product = ProductService.Current.Products.FirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                _logger.LogInformation("找不到id为{0}的产品", id);
                _mailService.Send("Product Deleted", $"Id为{id}的产品被删除了");
                return Json(product);
            }
            return new JsonResult(product);
        }
    }
}
