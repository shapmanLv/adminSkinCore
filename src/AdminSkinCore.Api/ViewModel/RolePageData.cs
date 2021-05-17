using AdminSkinCore.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ViewModel
{
    /// <summary>
    /// 角色分页数据  数据传输模型
    /// </summary>
    public class RolePageData
    {
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
        /// <summary>
        /// 角色数据
        /// </summary>
        public List<RoleInfo> RoleInfos { get; set; }
    }
}
