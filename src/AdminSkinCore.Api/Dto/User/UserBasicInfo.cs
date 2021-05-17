using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 用户基础信息
    /// </summary>
    public class UserBasicInfo
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 上一次的登录时间
        /// </summary>
        public DateTime? LastLoginTime { get; set; } = null;
        /// <summary>
        /// 上一次登录的IP地址
        /// </summary>
        public string LastLoginIp { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
