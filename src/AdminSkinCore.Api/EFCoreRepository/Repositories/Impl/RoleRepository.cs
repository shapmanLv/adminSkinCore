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
    /// 角色仓储
    /// </summary>
    public class RoleRepository : Repository<AdminSkinDbContext, Role, long> , IRoleRepository
    {
        public RoleRepository(AdminSkinDbContext context)
            : base(context)
        { }
    }
}
