using AdminSkinCore.Api.EFCoreRepository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 用户与角色
    /// </summary>
    public class UserRole : Entity<long>
    {
        /// <summary>
        /// 用户id
        /// </summary>
        [Required]
        public long UserId { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        [Required]
        public long RoleId { get; set; }
    }
}
