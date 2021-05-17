using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 批量删除角色 数据传输模型
    /// </summary>
    public class BatchRemoveRoleRequest
    {
        /// <summary>
        /// 角色id数组
        /// </summary>
        public long[] RoleIds { get; set; }
    }
}
