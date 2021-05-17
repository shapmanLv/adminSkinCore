using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 角色添加 数据传输模型
    /// </summary>
    public class AddRoleRequest
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 接口id数组
        /// </summary>
        public long[] authorizeApiIdIds { get; set; }
        /// <summary>
        /// 菜单id数组
        /// </summary>
        public long[] menuIds { get; set; }
    }
}
