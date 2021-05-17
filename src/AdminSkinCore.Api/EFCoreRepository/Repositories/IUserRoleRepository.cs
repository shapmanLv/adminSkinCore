using AdminSkinCore.Api.EFCoreRepository.Base;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Repositories
{
    /// <summary>
    /// 用户与角色仓储
    /// </summary>
    public interface IUserRoleRepository : IRepository<UserRole, long>
    {
    }
}
