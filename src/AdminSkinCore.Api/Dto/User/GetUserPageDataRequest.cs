using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 分页获取用户数据 数据传输模型
    /// </summary>
    public class GetUserPageDataRequest
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
        /// 名字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 登录名
        /// </summary>
        public string Account { get; set; }
    }
}
