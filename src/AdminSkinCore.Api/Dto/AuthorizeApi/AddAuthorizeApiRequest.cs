using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 添加接口时的数据传输对象
    /// </summary>
    public class AddAuthorizeApiRequest
    {
        /// <summary>
        /// 接口路径
        /// </summary>
        [Required]
        public string RouterPath { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; } = "";
    }
}
