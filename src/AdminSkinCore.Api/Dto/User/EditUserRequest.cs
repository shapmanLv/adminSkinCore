using AdminSkinCore.Api.Utility.CustomAttribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 用户编辑 数据传输模型
    /// </summary>
    public class EditUserRequest
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        [Required]
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
