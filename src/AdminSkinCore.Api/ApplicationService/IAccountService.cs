using AdminSkinCore.Api.Common;
using AdminSkinCore.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ApplicationService
{
    public interface IAccountService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<ResponseModel> Login(LoginRequest dto);
        /// <summary>
        /// 发送验证码
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        Task<ResponseModel> SendCaptcha(string account);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="originalPassword"></param>
        /// <param name="newPassword"></param>
        /// <returns></returns>
        Task<ResponseModel> UpdatePassword(string originalPassword, string newPassword);
    }
}
