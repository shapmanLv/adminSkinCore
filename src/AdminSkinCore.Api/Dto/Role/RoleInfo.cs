using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 角色信息
    /// </summary>
    public class RoleInfo : RoleBasicInfo
    {        
        /// <summary>
        /// 此角色管辖的接口
        /// </summary>
        public List<AuthorizeApiInfo> AuthorizeApiInfos { get; set; }
        /// <summary>
        /// 数据库中记录的与此角色绑定的菜单id
        /// </summary>
        public List<long> MenuIds { get; set; }
    }
}
