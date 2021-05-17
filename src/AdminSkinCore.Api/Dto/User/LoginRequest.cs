using System.ComponentModel.DataAnnotations;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 账户登录数据传输模型
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 六位数的数字验证码
        /// </summary>
        [StringLength(6)]
        [Required]
        public string Captcha { get; set; }
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public string ClientIp { get; set; }
    }
}
