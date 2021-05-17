using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.Dto
{
    /// <summary>
    /// 接口编辑的数据传输对象
    /// </summary>
    public class EditAuthorizeApiRequest
    {
        /// <summary>
        /// id 
        /// </summary>
        [Required]
        public long Id { get; set; }
        /// <summary>
        /// 接口路径
        /// </summary>
        [Required]
        public string RouterPath { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; }
    }
}
