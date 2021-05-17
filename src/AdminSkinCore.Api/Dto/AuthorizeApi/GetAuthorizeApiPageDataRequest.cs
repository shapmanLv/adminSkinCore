using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 分页获取需授权的api接口 数据传输模型
    /// </summary>
    public class GetAuthorizeApiPageDataRequest
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
        /// 路径
        /// </summary>
        public string RouterPath { get; set; }
    }
}
