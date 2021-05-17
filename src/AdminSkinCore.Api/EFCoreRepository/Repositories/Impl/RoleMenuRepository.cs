using AdminSkinCore.Api.EFCoreRepository.Base;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Repositories.Impl
{
    /// <summary>
    /// 角色菜单仓储
    /// </summary>
    public class RoleMenuRepository : Repository<AdminSkinDbContext, RoleMenu, long> , IRoleMenuRepository
    {
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        public RoleMenuRepository(AdminSkinDbContext context)
            : base(context) { }
    }
}
