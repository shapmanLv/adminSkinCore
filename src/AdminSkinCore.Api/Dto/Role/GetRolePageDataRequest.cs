using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 分页获取角色 数据传输模型
    /// </summary>
    public class GetRolePageDataRequest
    {
        /// <summary>
        /// 页码
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// 每页显示数量
        /// </summary>
        public int Pagesize { get; set; }
        /// <summary>
        /// 角色名
        /// </summary>
        public string Name { get; set; }
    }
}
