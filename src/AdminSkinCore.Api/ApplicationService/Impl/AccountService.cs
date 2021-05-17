using AdminSkinCore.Api.Common;
using AdminSkinCore.Api.Common.Sms;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Repositories;
using AutoMapper;
using AutoMapper.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ApplicationService.Impl
{
    /// <summary>
    /// 账户应用服务
    /// </summary>
    public class AccountService : IAccountService
    {
        /// <summary>
        /// 用户仓储
        /// </summary>
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// EF Core上下文
        /// </summary>
        private readonly AdminSkinDbContext _adminSkinDbContext;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _logger;
        /// <summary>
        /// 机器环境
        /// </summary>
        private readonly IWebHostEnvironment _env;
        /// <summary>
        /// 发送短信
        /// </summary>
        private readonly ISmsSender _smsSender;
        /// <summary>
        /// sha 加密
        /// </summary>
        private readonly IBCryptService _bCryptService;
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly IDistributedCache _cache;
        /// <summary>
        /// 系统配置读取对象
        /// </summary>
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        /// <summary>
        /// 请求上下文
        /// </summary>
        private readonly HttpContext _httpContext;

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="customBCrypt"></param>
        /// <param name="cache"></param>
        /// <param name="configuration"></param>
        /// <param name="log"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="adminSkinDbContext"></param>
        /// <param name="context"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="env"></param>
        /// <param name="smsSender"></param>
        public AccountService(
            IUserRepository userRepository,
            IBCryptService customBCrypt,
            IDistributedCache cache,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            ILogger<AccountService> logger,
            IHttpContextAccessor httpContextAccessor,
            AdminSkinDbContext adminSkinDbContext,
            IUserRoleRepository userRoleRepository,
            IMapper mapper,
            IWebHostEnvironment env,
            ISmsSender smsSender
           )
        {
            _userRepository = userRepository;
            _bCryptService = customBCrypt;
            _cache = cache;
            _configuration = configuration;
            _logger = logger;
            _httpContext = httpContextAccessor.HttpContext;
            _adminSkinDbContext = adminSkinDbContext;
            _mapper = mapper;
            _env = env;
            _smsSender = smsSender;
        }

        /// <summary>
        /// 登录
        /// </summary>                                         
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<ResponseModel> Login(LoginRequest dto)
        {
            /* 检查是否是有此账户 */
            var user = await _adminSkinDbContext.Users.FirstOrDefaultAsync(u => u.Account == dto.Account);
            if (user == null)
                return new ResponseModel { Code = 10001, Msg = "此用户不存在" };

            /* 检查账户是否是被禁用 */
            if (!user.Enabled)
                return new ResponseModel { Code = 10002, Msg = "此账户已被限制使用，请联系管理员" };

            /* 先从缓存中，检查冻结情况，及获取该用户的密码错误次数缓存 */
            int passwordErrorCount = await GetPasswordErrorCount(dto.Account);

            // 判断账户是否冻结
            if (passwordErrorCount >= int.Parse(_configuration["Settings:PasswordErrorLimitAsLogin"]))
                return new ResponseModel { Code = 10003, Msg = "因密码错误次数过多，账号已被冻结，请稍后再试" };

            /* 比对验证码 */
            var verificationCodeString = await _cache.GetStringAsync($"{UserService.CachePrefix.Captcha}:{dto.Account}");

            if (string.IsNullOrEmpty(verificationCodeString)) // 检查缓存中是否是没有验证码缓存
                return new ResponseModel { Code = 10004, Msg = "验证码已过期，请重新获取" };

            var temp = JsonConvert.DeserializeObject<Dictionary<string, string>>(verificationCodeString);
            var captcha = temp["code"];
            if (captcha != dto.Captcha) // 检查是否匹配
                return new ResponseModel { Code = 10005, Msg = "验证码错误" };

            /* 比对密码 */
            if (!_bCryptService.Verify(dto.Password, user.Password))
            {
                /* 用户密码错误，更新记录到缓存 */
                passwordErrorCount++;
                var maxErrorLimit = int.Parse(_configuration["Settings:PasswordErrorLimitAsLogin"]);
                if (passwordErrorCount < maxErrorLimit)
                {
                    var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(
                            TimeSpan.FromMinutes(int.Parse(_configuration["Settings:LogPasswordErrorCountTimeInterval"])));
                    await _cache.SetStringAsync($"{UserService.CachePrefix.LoginFail}:{user.Account.Trim()}", passwordErrorCount.ToString(), options);
                }
                else
                {
                    // 保持次数缓存的长时间留置，以达到冻结的效果
                    var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(
                        TimeSpan.FromHours(int.Parse(_configuration["Settings:AccountFrozenDuration"])));
                    await _cache.SetStringAsync($"{UserService.CachePrefix.LoginFail}:{user.Account.Trim()}", passwordErrorCount.ToString(), options);
                }

                return new ResponseModel { Code = 10006, Msg = "密码错误" };
            }

            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Account));
            identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));
            var roleRecords = (from userRole in _adminSkinDbContext.UserRoles.Where(u => u.UserId == user.Id)
                               join role in _adminSkinDbContext.Roles on userRole.RoleId equals role.Id
                               select role);
            foreach (var item in roleRecords)
            {
                if (item.Name.Trim() == _configuration["Settings:AdminRoleName"]) // 对于拥有 admin 角色的用户，存储一条特殊的Claim，表示为用户为超级管理员
                {
                    identity.AddClaim(new Claim("IsAdmin", item.Id.ToString()));
                    continue;
                }
                identity.AddClaim(new Claim(ClaimTypes.Role, item.Id.ToString()));
            }

            ClaimsPrincipal principal = new ClaimsPrincipal(identity);
            await _httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal,
                        new AuthenticationProperties()
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(2),//有效时间
                            AllowRefresh = true // 允许自动刷新
                        });

            /* 用户的登录痕迹，更新至数据库 */
            user.LastLoginIp = dto.ClientIp;
            user.LastLoginTime = DateTime.Now;
            user.IsLogin = true;
            await _adminSkinDbContext.SaveChangesAsync();

            await _cache.RemoveAsync($"{UserService.CachePrefix.AccountInfo}:{user.Account.Trim()}"); // 清除用户信息缓存
            await _cache.RemoveAsync($"{UserService.CachePrefix.LoginFail}:{user.Account.Trim()}"); // 清除错误次数缓存
            await _cache.RemoveAsync($"{UserService.CachePrefix.Captcha}:{user.Account.Trim()}"); // 清除验证码缓存
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="account">登录名</param>
        /// <returns></returns>
        public async Task<ResponseModel> SendCaptcha(string account)
        {
            var userRecord = await _adminSkinDbContext.Users.FirstOrDefaultAsync(u => u.Account == account); // 按照用户名获取用户记录
            if (userRecord == null)
                return new ResponseModel { Code = 10001, Msg = "此用户不存在" };

            int captcha = 0;
            /* 尝试从缓存中获取验证码 */
            var temp = await _cache.GetStringAsync($"{UserService.CachePrefix.Captcha}:{account}");
            if (!string.IsNullOrEmpty(temp))
            {
                var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(temp);
                var lastCodeCreateTime = DateTime.Parse(data["createtime"]);
                captcha = int.Parse(data["code"]);
                if (DateTime.Now.Subtract(lastCodeCreateTime).TotalSeconds < 50) // 上一次发送验证码的时间距离现在不到50s
                    return new ResponseModel { Code = 10002, Msg = "操作太频繁，请稍后再试" };
            }

            // 设置验证码过期时间
            var options = new DistributedCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(int.Parse(_configuration["Settings:VerificationCodeExpirationTime"])));

            // 测试环境与开发环境下，统一使用 123456 作为验证码
            if (_env.IsDevelopment() || _env.IsStaging())
                captcha = 123456;
            else
            {
                Random random = new Random();
                int i = 0;
                while (i++ < 10)
                {
                    var number = random.Next(100000, 999999);
                    if (captcha != number)
                    {
                        captcha = number;
                        break;
                    }
                }

                if (!await SmsSend(userRecord.PhoneNumber, captcha.ToString()))
                    return new ResponseModel() { Code = 10003, Msg = "短信发送失败" };
            }

            Dictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("code", captcha.ToString());
            keyValues.Add("createtime", DateTime.Now.ToString());
            // 把验证码及当前时间一起存入缓存
            await _cache.SetStringAsync($"{UserService.CachePrefix.Captcha}:{account}", JsonConvert.SerializeObject(keyValues), options);
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }

        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="phoneNumbers">手机号</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        private async Task<bool> SmsSend(string phoneNumbers, string code)
        {
            IDictionary<string, string> keyValues = new Dictionary<string, string>();
            keyValues.Add("code", code);
            (bool success, string response) = await _smsSender.Send(new SmsModel
            {
                AccessKeyId = _configuration["AliyunSms:AccessKeyID"],
                AccessKeySecret = _configuration["AliyunSms:AccessKeySecret"],
                PhoneNumbers = phoneNumbers,
                SignName = _configuration["AliyunSms:SignName"],
                TempletCode = _configuration["AliyunSms:TempletCode"],
                TemplateParam = keyValues
            });
            _logger.LogInformation("验证码发送后，返回的报文为：" + response);

            return success;
        }

        /// <summary>
        /// 获取用户在登录过程中的密码错误次数
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        private async Task<int> GetPasswordErrorCount(string account)
        {
            var intString = await _cache.GetStringAsync($"{UserService.CachePrefix.LoginFail}:{account.Trim()}");
            if (intString == null)
                return 0;

            return int.Parse(intString);
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        public async Task<ResponseModel> UpdatePassword(string originalPassword, string newPassword)
        {
            var userId = _httpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            if (userId.Value == null)
                return new ResponseModel<UserBasicInfo> { Code = 10001, Msg = "未找到用户信息，请尝试退出登录后重试" };

            var userInfo = await _adminSkinDbContext.Users.FirstOrDefaultAsync(u => u.Id == long.Parse(userId.Value));
            if (userInfo == null)
                return new ResponseModel<UserBasicInfo> { Code = 10002, Msg = "未找到用户信息" };

            if (!_bCryptService.Verify(originalPassword, userInfo.Password))
                return new ResponseModel() { Code = 10003, Msg = "原密码错误" };

            userInfo.Password = _bCryptService.HashPassword(newPassword);
            await _adminSkinDbContext.SaveChangesAsync();
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }
    }
}
