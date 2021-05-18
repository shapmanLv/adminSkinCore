using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSKinCore.Common.Extension.LinqExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.EFCore
{
    public class AdminSkinDbContextSeed
    {
        /// <summary>
        /// 系统配置读取
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// 用户密码加密服务
        /// </summary>
        private readonly IBCryptService _bCryptService;

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        /// <param name="bCryptService"></param>
        public AdminSkinDbContextSeed(IConfiguration configuration, ILogger<AdminSkinDbContextSeed> logger, IBCryptService bCryptService)
        {
            _configuration = configuration;
            _logger = logger;
            _bCryptService = bCryptService;
        }

        /// <summary>
        /// 迁移种子数据
        /// </summary>
        /// <param name="context">ef core db context</param>
        /// <param name="env">主机环境</param>
        /// <param name="logger">日志</param>
        /// <param name="retry">重试次数</param>
        /// <returns></returns>
        public async Task SeedDataMigrationAsync(AdminSkinDbContext context, IWebHostEnvironment env, int? retry = 0)
        {
            int retryForAvaiability = retry.Value;

            try
            {
                var contentRootPath = env.ContentRootPath;
                var webroot = env.WebRootPath;

                if(context.Menus.Any() is false) // 如果 menu 表里没有数据，则开始迁移menu表的种子数据
                {
                    _logger.LogInformation("{table}表中没有数据，准备迁移本地种子数据至数据库", "Menus");
                    await context.Menus.AddRangeAsync(
                        await GetModelFromFileAsync<Menu>(
                            Path.Combine(contentRootPath, "EFCoreRepository", "EFCore", "menuSeedData.csv"),
                            new []{ "Id", "Name", "RouterName", "RouterPath", "Desc", "Hidden", "ParentId", "ParentIdStr", "Sort", "Icon" } 
                        )
                    );
                }

                if (context.Users.Any() is false)
                {
                    _logger.LogInformation("{table}表中没有数据，准备迁移本地种子数据至数据库", "Users");
                    await context.Users.AddRangeAsync(
                        await GetModelFromFileAsync<User>(
                            Path.Combine(contentRootPath, "EFCoreRepository", "EFCore", "userSeedData.csv"),
                            new[] { "Id", "Account", "Enabled", "IsLogin", "Email", "Name", "PhoneNumber" },
                            (c, h) =>
                            {
                                var model = GenerateModelDefault<User>(c, h);
                                model.Password = _bCryptService.HashPassword("123"); // 统一设置，默认密码为 123
                                return model;
                            }
                        )
                    );
                }

                if (context.Roles.Any() is false)
                {
                    _logger.LogInformation("{table}表中没有数据，准备迁移本地种子数据至数据库", "Roles");
                    await context.Roles.AddRangeAsync(
                        await GetModelFromFileAsync<Role>(
                            Path.Combine(contentRootPath, "EFCoreRepository", "EFCore", "roleSeedData.csv"),
                            new[] { "Id", "Name", "Desc" }
                        )
                    );
                }

                if (context.UserRoles.Any() is false)
                {
                    _logger.LogInformation("{table}表中没有数据，准备迁移本地种子数据至数据库", "UserRoles");
                    await context.UserRoles.AddRangeAsync(
                        await GetModelFromFileAsync<UserRole>(
                            Path.Combine(contentRootPath, "EFCoreRepository", "EFCore", "userRoleSeedData.csv"),
                            new[] { "Id", "RoleId", "UserId" }
                        )
                    );
                }

                if (context.RoleMenus.Any() is false)
                {
                    _logger.LogInformation("{table}表中没有数据，准备迁移本地种子数据至数据库", "RoleMenus");
                    await context.RoleMenus.AddRangeAsync(
                        await GetModelFromFileAsync<RoleMenu>(
                            Path.Combine(contentRootPath, "EFCoreRepository", "EFCore", "roleMenuSeedData.csv"),
                            new[] { "Id", "RoleId", "MenuId" }
                        )
                    );
                }

                if (context.RoleAuthorizeApis.Any() is false)
                {
                    _logger.LogInformation("{table}表中没有数据，准备迁移本地种子数据至数据库", "RoleAuthorizeApis");
                    await context.RoleAuthorizeApis.AddRangeAsync(
                        await GetModelFromFileAsync<RoleAuthorizeApi>(
                            Path.Combine(contentRootPath, "EFCoreRepository", "EFCore", "roleAuthorizeApiSeedData.csv"),
                            new[] { "Id", "RoleId", "AuthorizeApiId" }
                        )
                    );
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                if (retryForAvaiability < 10)
                {
                    retryForAvaiability++;

                    _logger.LogError(ex, "当针对 {DbContextName} 准备迁移数据时发生错误，错误信息为：{exceptionMsg}", nameof(AdminSkinDbContextSeed), ex.Message);

                    await SeedDataMigrationAsync(context, env, retryForAvaiability);
                }
            }
        }

        /// <summary>
        /// 加载迁移文件，将迁移文件中的数据转换成 model
        /// </summary>
        /// <typeparam name="TModel">数据库数据模型，必须是一个带无参构造的类</typeparam>
        /// <param name="seedDataCsvFilePath">csv种子数据文件路径</param>
        /// <param name="requiredHeaders">csv文件表头校验规则（列举出要按照csv文件进行属性赋值的属性名）</param>
        /// <param name="genareteModelFunc">按照csv数据类和csv数据表头生成数据model的委托（可不填，默认使用反射的形式进行属性赋值，此默认的方法大部分情况均适用）</param>
        /// <returns></returns>
        private async Task<IEnumerable<TModel>> GetModelFromFileAsync<TModel>(string seedDataCsvFilePath, string[] requiredHeaders, Func<string[], string[], TModel> genareteModelFunc = null) where TModel : class , new()
        {
            // 检查种子数据文件是否存在
            if (!File.Exists(seedDataCsvFilePath)) 
            {
                _logger.LogError("在寻找种子数据文件的过程中，csv文件不存在，寻找路径为：{path}", seedDataCsvFilePath);
                return null;
            }

            /* 提取出表头，并在提取过程中进行一定的表头数据校验 */
            IEnumerable<string> csvFileheaders = null;
            try
            {
                csvFileheaders = GetHeadersFromCsvFile(requiredHeaders, seedDataCsvFilePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "csv文件在读取过程中出现错误，具体错误信息为: {Message}", ex.Message);

                return null;
            }

            var headers = csvFileheaders.ToArray();
            /* 从csv文件第二行开始逐行遍历，生成数据 */
            var models = (await File.ReadAllLinesAsync(seedDataCsvFilePath))
                        .Where(row => row.Trim() != "")
                        .Skip(1) // 跳过表头
                        .Select(row => Regex.Split(row, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)"))
                        .SelectTry(column => genareteModelFunc == null ? GenerateModelDefault<TModel>(column, headers) : genareteModelFunc(column, headers))
                        .OnCaughtException(ex => { _logger.LogError(ex, "csv文件读取发生错误，具体错误信息为: {Message}", ex.Message); return null; })
                        .Where(x => x != null);

            return models;
        }

        /// <summary>
        /// 默认的生成数据model的方法(反射的方式，按照表头进行寻找相同名字的属性进行赋值)
        /// </summary>
        /// <typeparam name="TModel">此数据模型必须是一个带空构造的类</typeparam>
        /// <param name="csvFileDataColumn">当前这一行数据中的数据列集合(csv文件的原始头，可修改集合里的表头值，但绝不可发生索引顺序的调换，例如排序)</param>
        /// <param name="headers">表头字段,用该字段定为数据列</param>
        /// <returns></returns>
        private static TModel GenerateModelDefault<TModel>(string[] csvFileDataColumn, string[] headers) where TModel : class , new()
        {
            if (csvFileDataColumn.Count() != headers.Count())
                throw new Exception($"在准备开始按照csv文件的数据生成model的时候，发现传进来的参数 csv文件表头长度 与 数据列的列数 不一致");

            var model = new TModel();
            var modelProps = typeof(TModel).GetRuntimeProperties();
            for (int i = 0; i < headers.Count(); i++)
            {
                var prop = modelProps.FirstOrDefault(u => u.Name == headers[i]);
                if (prop is null)
                    continue;

                // 找到了和表头名字相同的属性，把数据赋值给属性
                var temp = csvFileDataColumn[i].Trim('\"');
                switch (prop.PropertyType.Name)
                {
                    case nameof(Int32):
                        prop.SetValue(model, int.TryParse(temp, out int intTypeParseResult) ? intTypeParseResult : 0);
                        break;
                    case nameof(Int64):
                        prop.SetValue(model, long.TryParse(temp, out long longTypeParseResult) ? longTypeParseResult : 0);
                        break;
                    case nameof(Boolean):
                        const string bitStr = "01"; // bool在数据库中存储方式是 bit, 0 和 1
                        if(bitStr.IndexOf(temp) >= 0)
                        {
                            prop.SetValue(model, temp == "0" ? false : true);
                            break;
                        }

                        prop.SetValue(model, bool.TryParse(temp, out bool boolTypeParseResult) ? boolTypeParseResult : false);
                        break;
                    default:
                        prop.SetValue(model, temp);
                        break;
                }                
            }

            return model;
        }

        /// <summary>
        /// 从 csv 文件中提取表头(会除去表头字段两旁的引号)
        /// </summary>
        /// <param name="requiredHeaders"></param>
        /// <param name="csvfile"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetHeadersFromCsvFile(IEnumerable<string> requiredHeaders, string csvfile)
        {
            // 从文件首行提取出表头
            var csvFileHeaders = File.ReadLines(csvfile).First()?.Trim()?.Split(',')?.Select(u => u.Trim().Trim('\"').Trim())?.Where(u => string.IsNullOrEmpty(u) == false);

            /* 校验提取出来的表头  */
            // 校验从csv文件获得的表头是否为null或空
            if (csvFileHeaders is null || csvFileHeaders.Any() is false)
                throw new Exception($"该csv文件不存在有效表头，错误文件为：{csvfile}");

            // 对比两边的数量
            if (csvFileHeaders.Count() != requiredHeaders.Count())            
                throw new Exception("csv文件校验并未通过，csv文件表头的字段数量为{csvFileHeaders.Count()},设定的必须字段数量为{requiredHeaders.Count()}，两者之间并不相等，错误文件为：{csvfile}");                     

            // 循环遍历表头，一次校验csv文件中获得的表头是否与设定的完全字段一致
            foreach (var requiredHeader in requiredHeaders)            
                if (!csvFileHeaders.Contains(requiredHeader))
                    throw new Exception($"csv文件表头不存在 {csvfile} 字段，该文件为{csvfile}");
            

            return csvFileHeaders;
        }
    }
}
