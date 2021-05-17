using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 批量删除需授权的接口 数据传输模型
    /// </summary>
    public class BatchRemoveAuthorizeApiRequest
    {
        /// <summary>
        /// 需授权的接口记录id集合
        /// </summary>
        public long[] AuthorizeApiIds { get; set; }
    }
}
