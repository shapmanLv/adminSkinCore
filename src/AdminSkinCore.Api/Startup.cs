using AdminSkinCore.Api.Aop;
using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.ApplicationService.Impl;
using AdminSkinCore.Api.AutoMapper;
using AdminSkinCore.Api.Common.Sms;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Repositories;
using AdminSkinCore.Api.EFCoreRepository.Repositories.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.Elasticsearch;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace AdminSkinCore.Api
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env, IConfiguration configuration)
        {
            Configuration = configuration;
            Env = env;

            Log.Logger.Information("当前应用环境为：" + Env.EnvironmentName);
        }

        /// <summary>
        /// 项目环境配置操作
        /// </summary>
        public IConfiguration Configuration { get; }
        /// <summary>
        /// 主机环境变量
        /// </summary>
        public IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(
            #region filter
                options =>
                {
                    options.Filters.Add(typeof(GlobalActionFilter));

                    if (!Env.IsDevelopment())
                        options.Filters.Add(typeof(GlobalExceptionFilter));
                })
            #endregion
           
            #region newtonsoft
                .AddNewtonsoftJson(options =>
                {
                    //修改属性名称的序列化方式
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

                    //修改时间的序列化方式
                    options.SerializerSettings.Converters.Add(new IsoDateTimeConverter() { DateTimeFormat = "yyyy/MM/dd HH:mm:ss" });
                }
);
            #endregion

            #region swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AdminSkinCore.Api", Version = "v1" });
            });
            #endregion
       
            #region EF Core            
            services.AddDbContext<AdminSkinDbContext>(options =>
                     options.UseMySql(Configuration.GetConnectionString("AdminSkinMysqlDb"),
                     ServerVersion.AutoDetect(Configuration.GetConnectionString("AdminSkinMysqlDb")),
                     mySqlOptionsAction: options =>
                     {
                         options.MigrationsAssembly(typeof(Startup).GetTypeInfo().Assembly.GetName().Name);
                         options.EnableRetryOnFailure(maxRetryCount: 15, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                     }));
            #endregion

            #region redis cache

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = Configuration.GetConnectionString("Redis");
                options.InstanceName = "";
            });

            // cookies使用DataProtection机制加密，默认把key存放在所部署的机器上，
            // 当一个app部署到多个机器上，每个机器的key就不一致。
            // 以下代码把key统一放到redis上
            var redis = ConnectionMultiplexer.Connect(Configuration.GetConnectionString("Redis"));
            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

            #endregion

            #region automapper

            services.AddAutoMapper(typeof(CustomProfile));

            #endregion

            #region cookie authentication

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.HttpOnly = true; // 前端不可以访问并使用此cookie
                    options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;

                    options.EventsType = typeof(CustomCookieAuthenticationEvents); // 使用自己重写后的自定义处理逻辑
                });
            services.AddScoped<CustomCookieAuthenticationEvents>(); // 必需带这一句，否则上面的代码会报错

            #endregion

            #region service injection configuration

            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IBCryptService, BCryptService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthorizeApiService, AuthorizeApiService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IMenuService, MenuService>();

            #endregion

            #region repository injection configuration

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAuthorizeApiRepository, AuthorizeApiRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IRoleAuthorizeApiRepository, RoleApiRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IMenuRepository, MenuRepository>();
            services.AddScoped<IRoleMenuRepository, RoleMenuRepository>();

            #endregion

            #region other injection configuration

            services.AddHttpContextAccessor();
            services.AddScoped<ISmsSender, AliyunSmsSender>();

            #endregion

            services.BuildServiceProvider().GetService<IAuthorizeApiService>().BuildAuthorizeApis(); // 扫描所有需要授权才能使用的接口，存储至数据库
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "AdminSkinCore.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // 启用 cookie中间件
            app.UseCookiePolicy();

            // 启用权限检查
            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
