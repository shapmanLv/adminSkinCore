using AdminSkinCore.Api.EFCoreRepository.Base;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.EFCore
{
    /// <summary>
    /// 通用模板使用的EF
    /// </summary>
    public class AdminSkinDbContext : DbContext
    {
        public AdminSkinDbContext(DbContextOptions<AdminSkinDbContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().HasIndex(u => u.Account).IsUnique();

            #region 迁移时的种子数据

            string passwordCiphertext = BCrypt.Net.BCrypt.EnhancedHashPassword("123", BCrypt.Net.HashType.SHA384);

            #region authorizeApi 表

            builder.Entity<AuthorizeApi>().HasData(new AuthorizeApi
            {
                Id = 1,
                RouterPath = "/Test/CheckPermission",
                Desc = "用于测试角色进行接口权限测试",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            #endregion

            #region menu 表

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 1,
                Name = "权限设置",
                RouterName = "permissionGroup",
                RouterPath = "/permission",
                Desc = "设置系统菜单及接口与用户之间的联系",
                Hidden = false,
                ParentId = 0,
                ParentIdStr = "",
                Sort = 99,
                Icon = "fa fa-gavel",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 2,
                Name = "接口管理",
                RouterName = "authorizeApi",
                RouterPath = "/permission/authorizeApi",
                Desc = "后台需要授权使用的接口管理",
                Hidden = false,
                ParentId = 1,
                ParentIdStr = "1",
                Sort = 0,
                Icon = "fa fa-cloud",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 3,
                Name = "菜单管理",
                RouterName = "menu",
                RouterPath = "/permission/menu",
                Desc = "系统侧边栏级联菜单及前端vue-router动态路由",
                Hidden = false,
                ParentId = 1,
                ParentIdStr = "1",
                Sort = 0,
                Icon = "fa fa-list-ul",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 4,
                Name = "角色管理",
                RouterName = "role",
                RouterPath = "/permission/role",
                Desc = "角色的管理 及 接口、菜单与角色之间的绑定",
                Hidden = false,
                ParentId = 1,
                ParentIdStr = "1",
                Sort = 0,
                Icon = "fa fa-sitemap",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 5,
                Name = "用户管理",
                RouterName = "user",
                RouterPath = "/permission/user",
                Desc = "系统用户的管理 及 角色的分配",
                Hidden = false,
                ParentId = 1,
                ParentIdStr = "1",
                Sort = 0,
                Icon = "fa fa-user",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 6,
                Name = "系统测试",
                RouterName = "systemtestGroup",
                RouterPath = "/systemtest",
                Desc = "权限操作演示",
                Hidden = false,
                ParentId = 0,
                ParentIdStr = "",
                Sort = 0,
                Icon = "fa fa-coffee",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 7,
                Name = "测试1",
                RouterName = "test1",
                RouterPath = "/systemtest/test1",
                Desc = "测试有页面权限但无接口权限",
                Hidden = false,
                ParentId = 6,
                ParentIdStr = "6",
                Sort = 1,
                Icon = "fa fa-plug",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Menu>().HasData(new Menu
            {
                Id = 8,
                Name = "测试2",
                RouterName = "test2",
                RouterPath = "/systemtest/test2",
                Desc = "用于测试页面跳转",
                Hidden = false,
                ParentId = 6,
                ParentIdStr = "6",
                Sort = 0,
                Icon = "fa fa-plug",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            #endregion

            #region role 表

            builder.Entity<Role>().HasData(new Role
            {
                Id = 1,
                Name = "admin",
                Desc = "管理员角色",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Role>().HasData(new Role
            {
                Id = 2,
                Name = "system test",
                Desc = "测试角色，有测试页面1的权限，没有该页面的接口权限",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<Role>().HasData(new Role
            {
                Id = 3,
                Name = "api test",
                Desc = "有一个测试接口的权限，以及测试页面2的权限",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            #endregion

            #region role menu 和 role authorizeApi

            builder.Entity<RoleMenu>().HasData(new RoleMenu
            {
                Id = 1,
                RoleId = 2,
                MenuId = 7,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<RoleMenu>().HasData(new RoleMenu
            {
                Id = 2,
                RoleId = 3,
                MenuId = 8,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<RoleAuthorizeApi>().HasData(new RoleAuthorizeApi
            {
                Id = 1,
                RoleId = 3,
                AuthorizeApiId = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            #endregion

            #region user 和 user role

            builder.Entity<User>().HasData(new User
            {
                Id = 1,
                Account = "admin",
                Enabled = true,
                IsLogin = false,
                Email = "",
                LastLoginIp = "",
                LastLoginTime = null,
                Name = "管理员",
                Password = passwordCiphertext,
                PhoneNumber = "15770853552",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<User>().HasData(new User
            {
                Id = 2,
                Account = "test",
                Enabled = true,
                IsLogin = false,
                Email = "",
                LastLoginIp = "",
                LastLoginTime = null,
                Name = "测试用户",
                Password = passwordCiphertext,
                PhoneNumber = "15770853552",
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<UserRole>().HasData(new UserRole
            {
                Id = 1,
                RoleId = 1,
                UserId = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            builder.Entity<UserRole>().HasData(new UserRole
            {
                Id = 2,
                RoleId = 2,
                UserId = 2,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            });

            #endregion

            #endregion
        }

        public override int SaveChanges()
        {
            // get entries that are being Added or Updated
            var modifiedEntries = ChangeTracker.Entries()
                    .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity as Entity<long>;

                if (entry.State == EntityState.Added)
                {
                    entity.CreateTime = DateTime.Now;
                }

                entity.UpdateTime = DateTime.Now;
            }

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // get entries that are being Added or Updated
            var modifiedEntries = ChangeTracker.Entries()
                    .Where(x => (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entry in modifiedEntries)
            {
                var entity = entry.Entity as Entity<long>;

                if (entry.State == EntityState.Added)
                {
                    entity.CreateTime = DateTime.Now;
                }

                entity.UpdateTime = DateTime.Now;
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<AuthorizeApi> AuthorizeApis { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<RoleAuthorizeApi> RoleAuthorizeApis { get; set; }
        public DbSet<RoleMenu> RoleMenus { get; set; }
        public DbSet<Menu> Menus { get; set; }
    }
}
