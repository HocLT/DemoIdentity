using DemoIdentity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DemoIdentity.Data
{
    public class AppDbContext: IdentityDbContext<AppUser>
    {
        public DbSet<Category> Categories { set; get; }
        public DbSet<Post> Posts { set; get; }
        public DbSet<Product> Products { set; get; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // phần này sẽ xử lý việc loại bỏ tiền tố AspNet của bảng được sinh ra
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // builder.Model.GetEntityTypes(): lấy về các entities 
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                // lấy về table name trong entity
                string tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    // đổi lại tên table
                    entityType.SetTableName(tableName.Substring(6));    // bỏ 6 ký tự đầu là "AspNet"
                }
            }

            // insert dữ liệu mẫu sau khi tạo table
            builder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Product 1", Description = "Descriptio for Product 1", Price = 10000},
                new Product { Id = 2, Name = "Product 2", Description = "Descriptio for Product 2", Price = 16000 },
                new Product { Id = 3, Name = "Product 3", Description = "Descriptio for Product 3", Price = 50000 },
                new Product { Id = 4, Name = "Product 4", Description = "Descriptio for Product 4", Price = 120000 }
                );
        }
    }
}
