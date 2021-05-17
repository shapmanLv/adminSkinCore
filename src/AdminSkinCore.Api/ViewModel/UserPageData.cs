using AdminSkinCore.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ViewModel
{
    /// <summary>
    /// 用户分页数据
    /// </summary>
    public class UserPageData
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 用户信息数据
        /// </summary>
        public List<UserInfo> UserInfos { get; set; }
    }
}
