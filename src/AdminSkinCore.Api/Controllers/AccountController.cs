using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.Common;
using AdminSkinCore.Api.Common.CustomAttribute;
using AdminSkinCore.Api.Dto;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Controllers
{
    /// <summary>
    /// 账户服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        /// <summary>
        /// 账户应用服务
        /// </summary>
        private readonly IAccountService _accountService;

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="accountService"></param>
        public AccountController(IAccountService accountService)
            => _accountService = accountService;

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public async Task Logout()
            => await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        /// <summary>
        /// 发送手机验证码
        /// </summary>
        /// <param name="account">登录名</param>
        /// <returns></returns>
        [HttpPost("SendCaptcha/{account}")]
        public async Task<ResponseModel> SendCaptcha([NotNull] string account)
            => await _accountService.SendCaptcha(account);

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        public async Task<ResponseModel> Login([FromBody] LoginRequest req)
            => await _accountService.Login(req);

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Authorize]
        [DoNotCheckPermission]
        [HttpPut("UpdatePassword")]
        public async Task<ResponseModel> UpdatePassword([FromBody] UpdatePasswordRequest req)
            => await _accountService.UpdatePassword(req.OriginalPassword, req.NewPassword);
    }
}
