using AdminSkinCore.Api.EFCoreRepository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 角色
    /// </summary>
    public class Role : Entity<long>
    {
        /// <summary>
        /// 角色名
        /// </summary>
        [Required]        
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; } = "";
    }
}
