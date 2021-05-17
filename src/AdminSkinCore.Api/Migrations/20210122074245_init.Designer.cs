﻿// <auto-generated />
using System;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AdminSkinCore.Api.Migrations
{
    [DbContext(typeof(AdminSkinDbContext))]
    [Migration("20210122074245_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.AuthorizeApi", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Desc")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RouterPath")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("AuthorizeApis");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 243, DateTimeKind.Local).AddTicks(5245),
                            Desc = "用于测试角色进行接口权限测试",
                            RouterPath = "/Test/CheckPermission",
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 243, DateTimeKind.Local).AddTicks(5807)
                        });
                });

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.Menu", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Desc")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Hidden")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("Icon")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<long>("ParentId")
                        .HasColumnType("bigint");

                    b.Property<string>("ParentIdStr")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RouterName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("RouterPath")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<int>("Sort")
                        .HasColumnType("int");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Menus");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(7815),
                            Desc = "设置系统菜单及接口与用户之间的联系",
                            Hidden = false,
                            Icon = "fa fa-gavel",
                            Name = "权限设置",
                            ParentId = 0L,
                            ParentIdStr = "",
                            RouterName = "permissionGroup",
                            RouterPath = "/permission",
                            Sort = 99,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(7821)
                        },
                        new
                        {
                            Id = 2L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8101),
                            Desc = "后台需要授权使用的接口管理",
                            Hidden = false,
                            Icon = "fa fa-cloud",
                            Name = "接口管理",
                            ParentId = 1L,
                            ParentIdStr = "1",
                            RouterName = "authorizeApi",
                            RouterPath = "/permission/authorizeApi",
                            Sort = 0,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8103)
                        },
                        new
                        {
                            Id = 3L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8130),
                            Desc = "系统侧边栏级联菜单及前端vue-router动态路由",
                            Hidden = false,
                            Icon = "fa fa-list-ul",
                            Name = "菜单管理",
                            ParentId = 1L,
                            ParentIdStr = "1",
                            RouterName = "menu",
                            RouterPath = "/permission/menu",
                            Sort = 0,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8131)
                        },
                        new
                        {
                            Id = 4L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8153),
                            Desc = "角色的管理 及 接口、菜单与角色之间的绑定",
                            Hidden = false,
                            Icon = "fa fa-sitemap",
                            Name = "角色管理",
                            ParentId = 1L,
                            ParentIdStr = "1",
                            RouterName = "role",
                            RouterPath = "/permission/role",
                            Sort = 0,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8154)
                        },
                        new
                        {
                            Id = 5L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8174),
                            Desc = "系统用户的管理 及 角色的分配",
                            Hidden = false,
                            Icon = "fa fa-user",
                            Name = "用户管理",
                            ParentId = 1L,
                            ParentIdStr = "1",
                            RouterName = "user",
                            RouterPath = "/permission/user",
                            Sort = 0,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8176)
                        },
                        new
                        {
                            Id = 6L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8201),
                            Desc = "权限操作演示",
                            Hidden = false,
                            Icon = "fa fa-coffee",
                            Name = "系统测试",
                            ParentId = 0L,
                            ParentIdStr = "",
                            RouterName = "systemtestGroup",
                            RouterPath = "/systemtest",
                            Sort = 0,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8202)
                        },
                        new
                        {
                            Id = 7L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8246),
                            Desc = "测试有页面权限但无接口权限",
                            Hidden = false,
                            Icon = "fa fa-plug",
                            Name = "测试1",
                            ParentId = 6L,
                            ParentIdStr = "6",
                            RouterName = "test1",
                            RouterPath = "/systemtest/test1",
                            Sort = 1,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8248)
                        },
                        new
                        {
                            Id = 8L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8269),
                            Desc = "用于测试页面跳转",
                            Hidden = false,
                            Icon = "fa fa-plug",
                            Name = "测试2",
                            ParentId = 6L,
                            ParentIdStr = "6",
                            RouterName = "test2",
                            RouterPath = "/systemtest/test2",
                            Sort = 0,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8271)
                        });
                });

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.Role", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Desc")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(9950),
                            Desc = "管理员角色",
                            Name = "admin",
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(9954)
                        },
                        new
                        {
                            Id = 2L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(153),
                            Desc = "测试角色，有测试页面1的权限，没有该页面的接口权限",
                            Name = "system test",
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(154)
                        },
                        new
                        {
                            Id = 3L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(179),
                            Desc = "有一个测试接口的权限，以及测试页面2的权限",
                            Name = "api test",
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(181)
                        });
                });

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.RoleAuthorizeApi", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<long>("AuthorizeApiId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("RoleAuthorizeApis");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            AuthorizeApiId = 1L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(3470),
                            RoleId = 3L,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(3474)
                        });
                });

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.RoleMenu", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("MenuId")
                        .HasColumnType("bigint");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.ToTable("RoleMenus");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1697),
                            MenuId = 7L,
                            RoleId = 2L,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1701)
                        },
                        new
                        {
                            Id = 2L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1967),
                            MenuId = 8L,
                            RoleId = 3L,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1968)
                        });
                });

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.User", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<string>("Account")
                        .IsRequired()
                        .HasColumnType("varchar(255) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<bool>("Enabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsLogin")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("LastLoginIp")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<DateTime?>("LastLoginTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasMaxLength(11)
                        .HasColumnType("varchar(11) CHARACTER SET utf8mb4");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("Id");

                    b.HasIndex("Account")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            Account = "admin",
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8652),
                            Email = "",
                            Enabled = true,
                            IsLogin = false,
                            LastLoginIp = "",
                            Name = "管理员",
                            Password = "$2a$11$jgeVAnyQZzQl3gdC/vpd/eQ7MX/x.TEIxpezl90ey/PgD4NnpxIxe",
                            PhoneNumber = "15770853552",
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8657)
                        },
                        new
                        {
                            Id = 2L,
                            Account = "test",
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8871),
                            Email = "",
                            Enabled = true,
                            IsLogin = false,
                            LastLoginIp = "",
                            Name = "测试用户",
                            Password = "$2a$11$jgeVAnyQZzQl3gdC/vpd/eQ7MX/x.TEIxpezl90ey/PgD4NnpxIxe",
                            PhoneNumber = "15770853552",
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8873)
                        });
                });

            modelBuilder.Entity("AdminSkinCore.Api.EFCoreRepository.Entities.UserRole", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("RoleId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("UpdateTime")
                        .HasColumnType("datetime(6)");

                    b.Property<long>("UserId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.ToTable("UserRoles");

                    b.HasData(
                        new
                        {
                            Id = 1L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(394),
                            RoleId = 1L,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(398),
                            UserId = 1L
                        },
                        new
                        {
                            Id = 2L,
                            CreateTime = new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(575),
                            RoleId = 2L,
                            UpdateTime = new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(576),
                            UserId = 2L
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
