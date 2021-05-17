using AdminSkinCore.Api.EFCoreRepository.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 角色与菜单
    /// </summary>
    public class RoleMenu : Entity<long>
    {
        /// <summary>
        /// 角色id
        /// </summary>
        public long RoleId { get; set; }
        /// <summary>
        /// 菜单id
        /// </summary>
        public long MenuId { get; set; }
    }
}
