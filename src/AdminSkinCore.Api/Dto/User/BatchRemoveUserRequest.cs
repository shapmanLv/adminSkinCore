using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 批量删除用户 数据传输模型
    /// </summary>
    public class BatchRemoveUserRequest
    {
        /// <summary>
        /// 用户id数组
        /// </summary>
        public long[] UserIds { get; set; }
    }
}
