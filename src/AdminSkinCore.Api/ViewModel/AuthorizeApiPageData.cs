using AdminSkinCore.Api.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ViewModel
{
    /// <summary>
    /// 接口分页数据
    /// </summary>
    public class AuthorizeApiPageData
    {
        /// <summary>
        /// 接口数据
        /// </summary>
        public IEnumerable<AuthorizeApiInfo> AuthorizeApiInfos { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }
    }
}
