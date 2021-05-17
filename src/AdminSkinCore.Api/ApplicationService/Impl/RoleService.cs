using AutoMapper;
using AdminSkinCore.Api.ApplicationService.Base;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.EFCoreRepository.Repositories;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static AdminSkinCore.Api.ApplicationService.Impl.UserService;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.ApplicationService.Impl
{
    /// <summary>
    /// 角色应用服务
    /// </summary>
    public class RoleService : Service, IRoleService
    {
        /// <summary>
        /// 角色仓储
        /// </summary>
        private readonly IRoleRepository _roleRepository;
        /// <summary>
        /// 角色与接口仓储
        /// </summary>
        private readonly IRoleAuthorizeApiRepository _roleAuthorizeApiRepository;
        /// <summary>
        /// 接口授权仓储
        /// </summary>
        private readonly IAuthorizeApiRepository _authorizeApiRepository;
        /// <summary>
        /// 用户与角色仓储
        /// </summary>
        private readonly IUserRoleRepository _userRoleRepository;
        /// <summary>
        /// 角色与菜单仓储
        /// </summary>
        private readonly IRoleMenuRepository _roleMenuRepository;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly IDistributedCache _cache;
        /// <summary>
        /// EF Core上下文
        /// </summary>
        private readonly AdminSkinDbContext _adminSkinDbContext;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="roleRepository"></param>
        /// <param name="roleAuthorizeApiRepository"></param>
        /// <param name="authorizeApiRepository"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="roleMenuRepository"></param>
        /// <param name="cache"></param>
        /// <param name="mapper"></param>
        /// <param name="adminSkinDbContext"></param>
        public RoleService(AdminSkinDbContext context,
            IRoleRepository roleRepository,
            IRoleAuthorizeApiRepository roleAuthorizeApiRepository,
            IAuthorizeApiRepository authorizeApiRepository,
            IUserRoleRepository userRoleRepository,
            IRoleMenuRepository roleMenuRepository,
            IDistributedCache cache,
            IMapper mapper,
            AdminSkinDbContext adminSkinDbContext
            )
        {
            _roleRepository = roleRepository;
            _roleAuthorizeApiRepository = roleAuthorizeApiRepository;
            _authorizeApiRepository = authorizeApiRepository;
            _userRoleRepository = userRoleRepository;
            _roleMenuRepository = roleMenuRepository;
            _cache = cache;
            _mapper = mapper;
            _adminSkinDbContext = adminSkinDbContext;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="authorizeApiIds">角色下管辖的api接口记录id数组</param>
        /// <param name="menuIds">菜单id数组</param>
        /// <returns></returns>
        public async Task<ResponseModel> AddRole(Role role, long[] authorizeApiIds, long[] menuIds)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {
                if (await _adminSkinDbContext.Roles.Where(u => u.Name == role.Name.Trim()).CountAsync() > 0)
                    return new ResponseModel { Code = 10001, Msg = "此角色已存在" };

                await _roleRepository.AddAsync(role);
                List<RoleAuthorizeApi> roleApis = new List<RoleAuthorizeApi>();
                foreach (var apiId in authorizeApiIds)
                {
                    var temp = await _adminSkinDbContext.AuthorizeApis.FirstOrDefaultAsync(u => u.Id == apiId);
                    if (temp != null)
                        roleApis.Add(new RoleAuthorizeApi { AuthorizeApiId = apiId, RoleId = role.Id });
                }
                List<RoleMenu> roleMenus = new List<RoleMenu>();
                foreach (var menuId in menuIds)
                {
                    if (await _adminSkinDbContext.Menus.Where(u => u.Id == menuId).CountAsync() > 0)
                        roleMenus.Add(new RoleMenu { RoleId = role.Id, MenuId = menuId });
                }
                await _roleMenuRepository.BatchAddAsync(roleMenus);
                await _roleAuthorizeApiRepository.BatchAddAsync(roleApis);

                await transaction.CommitAsync();
                return ResponseModel.BuildResponse(PublicStatusCode.Success);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        public async Task<ResponseModel> RemoveRole(long id)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {
                await _roleRepository.RemoveAsync(id);
                await _userRoleRepository.BatchRemoveAsync(u => u.RoleId == id);
                await _roleAuthorizeApiRepository.BatchRemoveAsync(u => u.RoleId == id);
                await _roleMenuRepository.BatchRemoveAsync(u => u.RoleId == id);
                await transaction.CommitAsync();
                await _cache.RemoveAsync($"{CachePrefix.ApiResource}:{id}");
                return ResponseModel.BuildResponse(PublicStatusCode.Success);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="authorizeApiIds">角色下管辖的api接口记录id数组</param>
        /// <param name="menuIds">菜单id数组</param>
        /// <returns></returns>
        public async Task<ResponseModel> EditRole(Role role, long[] authorizeApiIds, long[] menuIds)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {
                var temp = await _adminSkinDbContext.Roles.FirstOrDefaultAsync(u => u.Id == role.Id);
                if (temp == null)
                    return new ResponseModel { Code = 10001, Msg = "此记录已不存在，无法修改" };

                temp.Desc = role.Desc;
                temp.Name = role.Name;
                await _adminSkinDbContext.SaveChangesAsync();

                #region RoleAuthorizeApi 记录的修改

                var oldRoleAuthorizeApiData = _adminSkinDbContext.RoleAuthorizeApis.AsNoTracking().Where(u => u.RoleId == role.Id);
                List<RoleAuthorizeApi> hasRemoveRoleAuthorizeApiData = new List<RoleAuthorizeApi>(); // 需要删除的数据
                List<RoleAuthorizeApi> hasAddRoleAuthorizeApiData = new List<RoleAuthorizeApi>(); // 需要添加到数据库的数据
                bool flag = false; // 循环过程中的标记
                foreach (var apiId in authorizeApiIds)
                {
                    foreach (var record in oldRoleAuthorizeApiData)
                    {
                        if (apiId == record.AuthorizeApiId)
                        {
                            flag = true; // 这个api id在原有的数据中存在
                            break;
                        }
                    }
                    if (!flag && await _adminSkinDbContext.AuthorizeApis.AsNoTracking().FirstOrDefaultAsync(u => u.Id == apiId) != null) // 修改的数据里面，有api id是原有的里面没有的
                        hasAddRoleAuthorizeApiData.Add(new RoleAuthorizeApi { AuthorizeApiId = apiId, RoleId = role.Id });

                    flag = false;
                }

                foreach (var record in oldRoleAuthorizeApiData)
                {
                    foreach (var apiId in authorizeApiIds)
                    {
                        if (apiId == record.AuthorizeApiId)
                        {
                            flag = true; // 原有的在修改后的api id中存在
                            break;
                        }
                    }
                    if (!flag) // 在修改的数据里不存在，表示要删除
                        hasRemoveRoleAuthorizeApiData.Add(record);

                    flag = false;
                }

                await _roleAuthorizeApiRepository.BatchAddAsync(hasAddRoleAuthorizeApiData);
                await _roleAuthorizeApiRepository.BatchRemoveAsync(hasRemoveRoleAuthorizeApiData);                

                #endregion

                #region RoleMenu 表的修改

                var oldRoleMenuData = _adminSkinDbContext.RoleMenus.Where(u => u.RoleId == role.Id);
                List<RoleMenu> hasRemoveRoleMenuData = new List<RoleMenu>();
                List<RoleMenu> hasAddRoleMemuData = new List<RoleMenu>();

                foreach (var menuId in menuIds)
                {
                    foreach (var record in oldRoleMenuData)
                    {
                        if (menuId == record.MenuId)
                        {
                            flag = true; // 这个api id在原有的数据中存在
                            break;
                        }
                    }
                    if (!flag && await _adminSkinDbContext.Menus.AsNoTracking().FirstOrDefaultAsync(u => u.Id == menuId) != null) // 修改的数据里面，有api id是原有的里面没有的
                        hasAddRoleMemuData.Add(new RoleMenu { MenuId = menuId, RoleId = role.Id });

                    flag = false;
                }

                foreach (var record in oldRoleMenuData)
                {
                    foreach (var menuId in menuIds)
                    {
                        if (menuId == record.MenuId)
                        {
                            flag = true; // 原有的在修改后的api id中存在
                            break;
                        }
                    }
                    if (!flag) // 在修改的数据里不存在，表示要删除
                        hasRemoveRoleMenuData.Add(record);

                    flag = false;
                }

                await _roleMenuRepository.BatchRemoveAsync(hasRemoveRoleMenuData);
                await _roleMenuRepository.BatchAddAsync(hasAddRoleMemuData);

                #endregion

                await transaction.CommitAsync();
                if (hasAddRoleAuthorizeApiData.Count > 0 || hasRemoveRoleAuthorizeApiData.Count > 0)
                    await _cache.RemoveAsync($"{CachePrefix.ApiResource}:{role.Id}");
                return ResponseModel.BuildResponse(PublicStatusCode.Success);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pagesize">每页显示的数量</param>
        /// <param name="name">角色名(模糊匹配)</param>
        /// <returns></returns>
        public async Task<ResponseModel<RolePageData>> GetRolePageData(int page, int pagesize, string name)
        {
            List<RoleInfo> roleData;
            int total = 0;
            if(string.IsNullOrEmpty(name.Trim()))
                roleData = await _roleRepository.GetPageData(page, pagesize, out total, u => true, u => u.Id, u => new RoleInfo() { Id = u.Id, Name = u.Name, Desc = u.Desc }, false).ToListAsync();
            else
                roleData = await _roleRepository.GetPageData(page, pagesize, out total, u => u.Name.Contains(name.Trim()), u => u.Id, u => new RoleInfo() { Id = u.Id, Name = u.Name, Desc = u.Desc }, false).ToListAsync();

            foreach (var item in roleData)
            {
                item.AuthorizeApiInfos = await (from roleapi in _adminSkinDbContext.RoleAuthorizeApis.Where(u => u.RoleId == item.Id)
                                       join api in _adminSkinDbContext.AuthorizeApis on roleapi.AuthorizeApiId equals api.Id
                                       select new AuthorizeApiInfo
                                       {
                                           Id = api.Id,
                                           RouterPath = api.RouterPath,
                                           Desc = api.Desc
                                       }).ToListAsync();
                item.MenuIds = await _adminSkinDbContext.RoleMenus.Where(u => u.RoleId == item.Id).Select(u => u.MenuId).ToListAsync();
            }
            return new ResponseModel<RolePageData> { Data = new RolePageData { RoleInfos = roleData, TotalCount = total }, Code = 200, Msg = "成功" };
        }

        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<RoleBasicInfo>>> GetAllRole()
        {
            return new ResponseModel<List<RoleBasicInfo>>
            {
                Code = 200,
                Msg = "成功",
                Data = _mapper.Map<List<RoleBasicInfo>>(await _adminSkinDbContext.Roles.ToListAsync())
            };
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">角色id数组</param>
        /// <returns></returns>
        public async Task<ResponseModel> BatchRemove(long[] ids)
        {
            using(var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    List<Role> roles = new List<Role>();
                    List<UserRole> userRoles = new List<UserRole>();
                    List<RoleAuthorizeApi> roleAuthorizeApis = new List<RoleAuthorizeApi>();
                    List<RoleMenu> roleMenus = new List<RoleMenu>();

                    foreach (var item in ids)
                    {
                        var roleRecord = await _adminSkinDbContext.Roles.FirstOrDefaultAsync(u => u.Id == item);
                        if (roleRecord != null)
                            roles.Add(roleRecord);

                        var roleAuthorizeApiRecord = await _adminSkinDbContext.RoleAuthorizeApis.FirstOrDefaultAsync(u => u.RoleId == item);
                        if (roleAuthorizeApiRecord != null)
                            roleAuthorizeApis.Add(roleAuthorizeApiRecord);

                        var roleMenuRecord = await _adminSkinDbContext.RoleMenus.FirstOrDefaultAsync(u => u.RoleId == item);
                        if (roleMenuRecord != null)
                            roleMenus.Add(roleMenuRecord);

                        var userRoleRecord = await _adminSkinDbContext.UserRoles.FirstOrDefaultAsync(u => u.RoleId == item);
                        if (userRoleRecord != null)
                            userRoles.Add(userRoleRecord);
                    }

                    await _userRoleRepository.BatchRemoveAsync(userRoles);
                    await _roleMenuRepository.BatchRemoveAsync(roleMenus);
                    await _roleAuthorizeApiRepository.BatchRemoveAsync(roleAuthorizeApis);
                    await _roleRepository.BatchRemoveAsync(roles);

                    await transaction.CommitAsync();
                    foreach (var item in ids)
                        await _cache.RemoveAsync($"{CachePrefix.ApiResource}:{item}");
                    return ResponseModel.BuildResponse(PublicStatusCode.Success);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
