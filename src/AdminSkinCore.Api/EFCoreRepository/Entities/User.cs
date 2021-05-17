using System;
using System.ComponentModel.DataAnnotations;
using AdminSkinCore.Api.EFCoreRepository.Base;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 用户
    /// </summary>
    public class User : Entity<long>
    {
        /// <summary>
        /// 登录名
        /// </summary>
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
        /// <summary>
        /// 上一次的登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; } = null;
        /// <summary>
        /// 上一次登录的IP地址
        /// </summary>
        public string LastLoginIp { get; set; } = "";
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; } = "";
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; } = "";
        /// <summary>
        /// 该账户是否被启用
        /// </summary>
        [Required]
        public bool Enabled { get; set; } = true;
        /// <summary>
        /// 是否已经登录
        /// </summary>
        [Required]
        public bool IsLogin { get; set; } = false;
    }
}
