using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 角色编辑 数据传输对象
    /// </summary>
    public class EditRoleRequest : AddRoleRequest
    {
        /// <summary>
        /// 主键id
        /// </summary>
        public long Id { get; set; }
    }
}
