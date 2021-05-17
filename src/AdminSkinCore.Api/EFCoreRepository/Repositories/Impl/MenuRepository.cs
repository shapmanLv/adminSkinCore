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
    /// 菜单仓储
    /// </summary>
    public class MenuRepository : Repository<AdminSkinDbContext, Menu, long> , IMenuRepository
    {
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        public MenuRepository(AdminSkinDbContext context)
            : base(context) { }
    }
}
