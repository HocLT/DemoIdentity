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
        }
    }
}
