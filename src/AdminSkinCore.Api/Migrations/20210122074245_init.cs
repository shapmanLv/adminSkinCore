using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminSkinCore.Api.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuthorizeApis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RouterPath = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Desc = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorizeApis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ParentId = table.Column<long>(type: "bigint", nullable: false),
                    ParentIdStr = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    RouterPath = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    RouterName = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Desc = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Icon = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Hidden = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Sort = table.Column<int>(type: "int", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleAuthorizeApis",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    AuthorizeApiId = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleAuthorizeApis", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMenus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    MenuId = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMenus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Desc = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    RoleId = table.Column<long>(type: "bigint", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Account = table.Column<string>(type: "varchar(255) CHARACTER SET utf8mb4", nullable: false),
                    Name = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    Password = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: false),
                    LastLoginTime = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    LastLoginIp = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    PhoneNumber = table.Column<string>(type: "varchar(11) CHARACTER SET utf8mb4", maxLength: 11, nullable: false),
                    Email = table.Column<string>(type: "longtext CHARACTER SET utf8mb4", nullable: true),
                    Enabled = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsLogin = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    CreateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdateTime = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "AuthorizeApis",
                columns: new[] { "Id", "CreateTime", "Desc", "RouterPath", "UpdateTime" },
                values: new object[] { 1L, new DateTime(2021, 1, 22, 15, 42, 45, 243, DateTimeKind.Local).AddTicks(5245), "用于测试角色进行接口权限测试", "/Test/CheckPermission", new DateTime(2021, 1, 22, 15, 42, 45, 243, DateTimeKind.Local).AddTicks(5807) });

            migrationBuilder.InsertData(
                table: "Menus",
                columns: new[] { "Id", "CreateTime", "Desc", "Hidden", "Icon", "Name", "ParentId", "ParentIdStr", "RouterName", "RouterPath", "Sort", "UpdateTime" },
                values: new object[,]
                {
                    { 1L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(7815), "设置系统菜单及接口与用户之间的联系", false, "fa fa-gavel", "权限设置", 0L, "", "permissionGroup", "/permission", 99, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(7821) },
                    { 2L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8101), "后台需要授权使用的接口管理", false, "fa fa-cloud", "接口管理", 1L, "1", "authorizeApi", "/permission/authorizeApi", 0, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8103) },
                    { 3L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8130), "系统侧边栏级联菜单及前端vue-router动态路由", false, "fa fa-list-ul", "菜单管理", 1L, "1", "menu", "/permission/menu", 0, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8131) },
                    { 4L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8153), "角色的管理 及 接口、菜单与角色之间的绑定", false, "fa fa-sitemap", "角色管理", 1L, "1", "role", "/permission/role", 0, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8154) },
                    { 5L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8174), "系统用户的管理 及 角色的分配", false, "fa fa-user", "用户管理", 1L, "1", "user", "/permission/user", 0, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8176) },
                    { 6L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8201), "权限操作演示", false, "fa fa-coffee", "系统测试", 0L, "", "systemtestGroup", "/systemtest", 0, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8202) },
                    { 7L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8246), "测试有页面权限但无接口权限", false, "fa fa-plug", "测试1", 6L, "6", "test1", "/systemtest/test1", 1, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8248) },
                    { 8L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8269), "用于测试页面跳转", false, "fa fa-plug", "测试2", 6L, "6", "test2", "/systemtest/test2", 0, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(8271) }
                });

            migrationBuilder.InsertData(
                table: "RoleAuthorizeApis",
                columns: new[] { "Id", "AuthorizeApiId", "CreateTime", "RoleId", "UpdateTime" },
                values: new object[] { 1L, 1L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(3470), 3L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(3474) });

            migrationBuilder.InsertData(
                table: "RoleMenus",
                columns: new[] { "Id", "CreateTime", "MenuId", "RoleId", "UpdateTime" },
                values: new object[,]
                {
                    { 2L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1967), 8L, 3L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1968) },
                    { 1L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1697), 7L, 2L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(1701) }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CreateTime", "Desc", "Name", "UpdateTime" },
                values: new object[,]
                {
                    { 1L, new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(9950), "管理员角色", "admin", new DateTime(2021, 1, 22, 15, 42, 45, 245, DateTimeKind.Local).AddTicks(9954) },
                    { 2L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(153), "测试角色，有测试页面1的权限，没有该页面的接口权限", "system test", new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(154) },
                    { 3L, new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(179), "有一个测试接口的权限，以及测试页面2的权限", "api test", new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(181) }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "CreateTime", "RoleId", "UpdateTime", "UserId" },
                values: new object[,]
                {
                    { 1L, new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(394), 1L, new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(398), 1L },
                    { 2L, new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(575), 2L, new DateTime(2021, 1, 22, 15, 42, 45, 247, DateTimeKind.Local).AddTicks(576), 2L }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Account", "CreateTime", "Email", "Enabled", "IsLogin", "LastLoginIp", "LastLoginTime", "Name", "Password", "PhoneNumber", "UpdateTime" },
                values: new object[,]
                {
                    { 1L, "admin", new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8652), "", true, false, "", null, "管理员", "$2a$11$jgeVAnyQZzQl3gdC/vpd/eQ7MX/x.TEIxpezl90ey/PgD4NnpxIxe", "15770853552", new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8657) },
                    { 2L, "test", new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8871), "", true, false, "", null, "测试用户", "$2a$11$jgeVAnyQZzQl3gdC/vpd/eQ7MX/x.TEIxpezl90ey/PgD4NnpxIxe", "15770853552", new DateTime(2021, 1, 22, 15, 42, 45, 246, DateTimeKind.Local).AddTicks(8873) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Account",
                table: "Users",
                column: "Account",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthorizeApis");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "RoleAuthorizeApis");

            migrationBuilder.DropTable(
                name: "RoleMenus");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
