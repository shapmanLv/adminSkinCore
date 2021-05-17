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
    /// 用户与角色仓储
    /// </summary>
    public class UserRoleRepository : Repository<AdminSkinDbContext, UserRole, long> , IUserRoleRepository
    {
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        public UserRoleRepository(AdminSkinDbContext context)
            : base(context)
        { }
    }
}
