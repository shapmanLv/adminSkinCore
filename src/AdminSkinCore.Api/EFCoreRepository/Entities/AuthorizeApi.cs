using AdminSkinCore.Api.EFCoreRepository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 需授权的接口
    /// </summary>
    public class AuthorizeApi : Entity<long>
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
