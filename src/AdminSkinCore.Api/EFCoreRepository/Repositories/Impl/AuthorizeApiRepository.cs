using AdminSkinCore.Api.EFCoreRepository.Base;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Repositories.Impl
{
    /// <summary>
    /// api接口仓储
    /// </summary>
    public class AuthorizeApiRepository : Repository<AdminSkinDbContext, AuthorizeApi, long> , IAuthorizeApiRepository
    {
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        public AuthorizeApiRepository(AdminSkinDbContext context)
            : base(context)
        { }
    }
}
