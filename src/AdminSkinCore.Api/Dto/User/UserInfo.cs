using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo : UserBasicInfo
    {
        /// <summary>
        /// 角色信息
        /// </summary>
        public List<RoleBasicInfo> RoleInfos {get;set;}
    }
}
