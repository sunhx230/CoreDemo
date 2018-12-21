using CoreDemo.EntityConfiguration;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.Entities
{
    public class MyContext: DbContext
    {

        public MyContext(DbContextOptions<MyContext> options) : base(options)
        {
            //如果有数据库存在，那么什么也不会发生。但是如果没有，那么就会创建一个数据库。
            //但是运行到这段代码的话，并不会创建数据库，因为没有创建MyContext的实例，也就不会调用Constructor里面的内容。
            //Database.EnsureCreated();
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //下面代码也可以设置EFCore连接字符串
            //optionsBuilder.UseSqlServer("Data Source=.; Initial Catalog=EFCoreTest; User ID=sa; Password=sunhx230; Pooling=true");
            //base.OnConfiguring(optionsBuilder);  
        }

        /*
         * 开发过程中肯定会对一些entitiy进行一些修改就需要对数据表进行迁移（Migration） 官方提供了两种方式Data Annotation 和 Fluet Api
         *  Fluet Api
         *  当我们创建entity 就需要把他映射成为一个数据库的表,所以针对一些属性设置精度。长短。大小。 类型
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //设置Product映射成数据库表的相关设置
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
        }
        public DbSet<Product> Products { get; set; }
    }
}
