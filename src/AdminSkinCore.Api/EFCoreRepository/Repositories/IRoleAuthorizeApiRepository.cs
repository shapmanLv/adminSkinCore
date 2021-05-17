using AdminSkinCore.Api.EFCoreRepository.Base;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Repositories
{
    /// <summary>
    /// 角色与接口仓储
    /// </summary>
    public interface IRoleAuthorizeApiRepository : IRepository<RoleAuthorizeApi, long>
    {
    }
}
