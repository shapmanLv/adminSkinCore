using AdminSkinCore.Api.Utility.CustomAttribute;
using System.ComponentModel.DataAnnotations;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 新用户
    /// </summary>
    public class AddUserRequest
    {
        /// <summary>
        /// 登录名
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        public long[] RoleIds { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Required]
        [StringLength(11)]
        [Mobile]
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [Email]
        public string Email { get; set; }
    }
}
