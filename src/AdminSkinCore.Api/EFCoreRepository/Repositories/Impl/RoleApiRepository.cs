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
    /// 角色与接口仓储
    /// </summary>
    public class RoleApiRepository : Repository<AdminSkinDbContext, RoleAuthorizeApi, long> , IRoleAuthorizeApiRepository
    {
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        public RoleApiRepository(AdminSkinDbContext context)
            : base(context)
        { }
    }
}
