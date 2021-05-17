using AdminSkinCore.Api.Common;
using AdminSkinCore.Api.Common.CustomAttribute;
using AdminSkinCore.Api.Common.Helper;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static AdminSkinCore.Api.ApplicationService.Impl.UserService;

namespace AdminSkinCore.Api.Aop
{
    /// <summary>
    /// 基于 cookie的身份验证 自定义重写
    /// </summary>
    public class CustomCookieAuthenticationEvents : CookieAuthenticationEvents
    {
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly IDistributedCache _cache;
        /// <summary>
        /// EF上下文
        /// </summary>
        private readonly AdminSkinDbContext _adminSkinDbContext;
        /// <summary>
        /// 返回的状态码
        /// </summary>
        private int responseCode = 403;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="context"></param>
        public CustomCookieAuthenticationEvents(IDistributedCache cache, AdminSkinDbContext context)
        {
            _cache = cache;
            _adminSkinDbContext = context;
        }

        /// <summary>
        /// 自定义 Cookie 验证的处理逻辑
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            var accountEnabled = false;
            var accountIsLogin = false;
            var account = context.Principal.Claims.Where(u => u.Type == ClaimTypes.Name).FirstOrDefault()?.Value;
            var userInfoString = await _cache.GetStringAsync($"{CachePrefix.AccountInfo}:{account.Trim()}");
            // 缓存没有找到就从数据库中取
            if (userInfoString == null)
            {
                var userRecord = await _adminSkinDbContext.Users
                                                .AsNoTracking()
                                                .FirstOrDefaultAsync(u => u.Account == account.Trim());
                if (userRecord == null)
                    throw new SysException("cookies校验用户身份时，没有从数据库找到相应用户");

                accountEnabled = userRecord.Enabled;
                accountIsLogin = userRecord.IsLogin;
                Dictionary<string, string> keyValues = new Dictionary<string, string>();
                keyValues.Add(nameof(User.Enabled), userRecord.Enabled.ToString());
                keyValues.Add(nameof(User.IsLogin), userRecord.IsLogin.ToString());
                await _cache.SetStringAsync($"{CachePrefix.AccountInfo}:{account.Trim()}", JsonConvert.SerializeObject(keyValues),
                    new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
            }
            else
            {
                var temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(userInfoString);
                accountEnabled = bool.Parse(temp[nameof(User.Enabled)]);
                accountIsLogin = bool.Parse(temp[nameof(User.IsLogin)]);
            }


            if (accountEnabled == false) // 检查账户是否被禁用
            {
                context.RejectPrincipal();
                return;
            }

            if (accountIsLogin == false) // 检查账户是否登录
            {
                context.RejectPrincipal();
                return;
            }

            if (!await CheckPermission(context)) // 检查Api资源使用权限
            {
                responseCode = 401;
                context.RejectPrincipal();
            }
        }

        /// <summary>
        /// 跳转至登录页面事件处理程序
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override Task RedirectToLogin(RedirectContext<CookieAuthenticationOptions> context)
        {
            context.Response.StatusCode = responseCode;
            return Task.CompletedTask;
        }

        /// <summary>
        /// 权限检查
        /// </summary>
        /// <param name="principalContext"></param>
        /// <returns></returns> 
        private async Task<bool> CheckPermission(CookieValidatePrincipalContext principalContext)
        {
            /* 提取用户的行为，他想请求哪个Controller，哪个Action */
            var endpoint = principalContext.HttpContext.Features.Get<IEndpointFeature>().Endpoint;
            if (endpoint == null) 
                return false;
            var endpointMateDataCollection = endpoint.Metadata;
            if (endpointMateDataCollection == null)
                return true;

            /* 控制器或方法上打了 Authrize 标签的，才要继续进行接口权限校验 */
            if (endpointMateDataCollection.Where(u => u.GetType() == typeof(AuthorizeAttribute)).Count() == 0)
                return true;

            /* 检查controller 或者是 action 上面有没有打 DoNotCheckPermission 标签，有就不继续做权限校验了  */
            if (endpointMateDataCollection.Where(u => u.GetType() == typeof(DoNotCheckPermissionAttribute)).Count() > 0)
                return true;

            var controllerActionInfo = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>();
            if (controllerActionInfo == null)
                return false;
            string controllerName = controllerActionInfo.ControllerName;
            string actionName = controllerActionInfo.ActionName;

            /* 检查 httpcontext 的Claim中有没有 IsAdmin这一项 */
            var tryFindIsAdminClaim = principalContext.Principal.Claims.FirstOrDefault(u => u.Type == "IsAdmin");
            if (tryFindIsAdminClaim != null) // 此用户是管理员，直接返回 true
                return true;

            string notAiResourceLabel = "no"; // 当角色没有api资源时，缓存中将存储这个字符串
            List<string> apiResource = null; // 某角色下的Api资源

            // 从httpcontext中提取用户登录的时候存储进去的用户角色信息
            var userRoleClaims = principalContext.Principal.Claims.Where(u => u.Type == ClaimTypes.Role);
            foreach (var userRole in userRoleClaims)
            {
                string apiResourceJsonStr = await _cache.GetStringAsync($"{CachePrefix.ApiResource}:{userRole.Value}");

                if (apiResourceJsonStr != null && apiResourceJsonStr == notAiResourceLabel) // 缓存中有，但是该角色没有 APIResource
                    continue;                      // 分配给这个用户的 api 在接口管理页面给删掉了的情况下，就存在着没有 ApiResource的情形

                if (apiResourceJsonStr == null) // 缓存中没有
                {
                    long roleId = long.Parse(userRole.Value);
                    var apiData = await (from ra in _adminSkinDbContext.RoleAuthorizeApis.Where(u => u.RoleId == roleId)
                                         join a in _adminSkinDbContext.AuthorizeApis on ra.AuthorizeApiId equals a.Id
                                         select a.RouterPath).ToListAsync();
                    await _cache.SetStringAsync($"{CachePrefix.ApiResource}:{userRole.Value}",
                        apiData.Count() > 0 ? JsonConvert.SerializeObject(apiData) : "", //notAiResourceLabel,
                        new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(1)));
                    apiResource = apiData;
                }
                else // 缓存中有
                    apiResource = JsonConvert.DeserializeObject<List<string>>(apiResourceJsonStr);

                if (apiResource.Any(u => u == $"/{controllerName}/{actionName}")) // 匹配
                    return true;
            }

            return false;
        }
    }
}
