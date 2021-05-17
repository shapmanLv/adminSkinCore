using AdminSkinCore.Api.EFCoreRepository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 角色与接口
    /// </summary>
    public class RoleAuthorizeApi : Entity<long>
    {
        /// <summary>
        /// 角色id
        /// </summary>
        [Required]
        public long RoleId { get; set; }
        /// <summary>
        /// 接口id
        /// </summary>
        [Required]
        public long AuthorizeApiId { get; set; }
    }
}
