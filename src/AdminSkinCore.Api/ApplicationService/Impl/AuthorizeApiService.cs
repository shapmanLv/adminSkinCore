using AutoMapper;
using AdminSkinCore.Api.ApplicationService.Base;
using AdminSkinCore.Api.Common.CustomAttribute;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.EFCoreRepository.Repositories;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.ApplicationService.Impl
{
    /// <summary>
    /// 需授权的api接口应用服务
    /// </summary>
    public class AuthorizeApiService : Service, IAuthorizeApiService
    {
        /// <summary>
        /// 需授权的接口仓储
        /// </summary>
        private readonly IAuthorizeApiRepository _authorizeApiRepository;
        /// <summary>
        /// 角色与需授权接口
        /// </summary>
        private readonly IRoleAuthorizeApiRepository _roleAuthorizeApiRepository;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// EF Core上下文
        /// </summary>
        private readonly AdminSkinDbContext _adminSkinDbContext;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authorizeApiRepository"></param>
        /// <param name="roleAuthorizeApiRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="adminSkinDbContext"></param>
        public AuthorizeApiService(AdminSkinDbContext context, 
            IAuthorizeApiRepository authorizeApiRepository,
            IRoleAuthorizeApiRepository roleAuthorizeApiRepository,
            IMapper mapper,
            AdminSkinDbContext adminSkinDbContext
            )
        {
            _authorizeApiRepository = authorizeApiRepository;
            _roleAuthorizeApiRepository = roleAuthorizeApiRepository;
            _mapper = mapper;
            _adminSkinDbContext = adminSkinDbContext;
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddAuthorizeApi([NotNull] AuthorizeApi api)
        {
            if (await _adminSkinDbContext.AuthorizeApis.Where(u => u.RouterPath == api.RouterPath).CountAsync() > 0)
                return new ResponseModel { Code = 10001, Msg = "此接口已存在" };

            await _authorizeApiRepository.AddAsync(api);
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">接口记录的id</param>
        /// <returns></returns>
        public async Task<ResponseModel> RemoveAuthorizeApi(long id)
        {
            using(var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await _authorizeApiRepository.RemoveAsync(id);
                    var temp = await _adminSkinDbContext.RoleAuthorizeApis.Where(u => u.AuthorizeApiId == id).ToListAsync();
                    if (temp != null)
                        await _roleAuthorizeApiRepository.BatchRemoveAsync(temp);
                    await transaction.CommitAsync();
                    return ResponseModel.BuildResponse(PublicStatusCode.Success);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditAuthorizeApi(AuthorizeApi api)
        {
            var temp = await _adminSkinDbContext.AuthorizeApis.FirstOrDefaultAsync(u => u.Id == api.Id);
            if (temp == null)
                return new ResponseModel { Code = 10001, Msg = "不存在此数据，无法修改" };

            temp.RouterPath = api.RouterPath;
            temp.Desc = api.Desc;
            await _adminSkinDbContext.SaveChangesAsync();
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }

        /// <summary>
        /// 分页获取需授权的api数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pagesize">面页显示的数量</param>
        /// <param name="routerPath">数据筛选条件：接口路径</param>
        /// <returns></returns>        
        public async Task<ResponseModel<AuthorizeApiPageData>> GetAuthorizeApiPageData(int page, int pagesize, string routerPath)
        {
            IQueryable<AuthorizeApiInfo> temp;
            int total = 0;
            if(routerPath.Trim() == "")
                temp = _authorizeApiRepository.GetPageData(page, pagesize, out total, u => true, u => u.RouterPath.Trim('/'), 
                    u => new AuthorizeApiInfo { Id = u.Id, RouterPath = u.RouterPath, Desc = u.Desc }, true);
            else
                temp = _authorizeApiRepository.GetPageData(page, pagesize, out total, u => u.RouterPath.Contains(routerPath.Trim()), u => u.RouterPath.Trim('/'),
                    u => new AuthorizeApiInfo { Id = u.Id, RouterPath = u.RouterPath, Desc = u.Desc }, true);
            AuthorizeApiPageData apiPageData = new AuthorizeApiPageData();

            apiPageData.AuthorizeApiInfos = await temp?.ToListAsync();
            apiPageData.TotalCount = total;
            return new ResponseModel<AuthorizeApiPageData> { Data = apiPageData, Code = 200, Msg = "成功" };
        }

        /// <summary>
        /// 查询所有需授权的api接口
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<AuthorizeApiInfo>>> GetAllAuthorizeApi()
        {
            return new ResponseModel<List<AuthorizeApiInfo>>
            {
                Code = 200,
                Msg = "成功",
                Data = _mapper.Map<List<AuthorizeApiInfo>>(await _adminSkinDbContext.AuthorizeApis.OrderBy(u => u.RouterPath.Trim('/')).ToListAsync())
            };
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ResponseModel> BatchRemove(long[] ids)
        {
            using(var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync())
            {
                try
                { 
                    List<AuthorizeApi> authorizeApis = new List<AuthorizeApi>();
                    List<RoleAuthorizeApi> roleAuthorizeApis = new List<RoleAuthorizeApi>();
                    foreach (var item in ids)
                    {
                        /* 清除 Authorize表记录 */
                        var authorizeApiRecord = await _adminSkinDbContext.AuthorizeApis.FirstOrDefaultAsync(u => u.Id == item);
                        if (authorizeApiRecord == null)
                            continue;
                        authorizeApis.Add(authorizeApiRecord);
                        /* 清除 RoleAuthorize表的记录 */
                        var roleAuthorizeApiRecords = await _adminSkinDbContext.RoleAuthorizeApis.Where(u => u.AuthorizeApiId == item).ToListAsync();
                        if (roleAuthorizeApiRecords != null)
                            roleAuthorizeApis.AddRange(roleAuthorizeApiRecords);
                    }

                    await _authorizeApiRepository.BatchRemoveAsync(authorizeApis);
                    await _roleAuthorizeApiRepository.BatchRemoveAsync(roleAuthorizeApis);
                    await transaction.CommitAsync();
                    return ResponseModel.BuildResponse(PublicStatusCode.Success);
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
            
        }

        /// <summary>
        /// 扫描所有action，生成AuthorizeApi记录
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<int>> BuildAuthorizeApis()
        {
            var controllerTypes = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(u => u.GetTypes().Where(t => t.BaseType == typeof(ControllerBase)));
            List<AuthorizeApi> apis = new List<AuthorizeApi>();
            foreach (var item in controllerTypes)
            {
                // 直接在控制器上打 Authrize 标签，表示所有action都是要授权才可使用的
                bool saveAll = item.GetCustomAttribute<AuthorizeAttribute>() != null;

                foreach (var method in item.GetMethods())
                {
                    if (!method.GetCustomAttributes().Any(u => u.GetType().BaseType == typeof(HttpMethodAttribute)))
                        continue;

                    if ((saveAll || method.GetCustomAttribute<AuthorizeAttribute>() != null) && method.GetCustomAttribute<DoNotCheckPermissionAttribute>() == null)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.AppendFormat("/{0}/{1}", item.Name.Substring(0, item.Name.IndexOf(nameof(Controller))), method.Name);
                        string routerPath = builder.ToString();
                        if (await _adminSkinDbContext.AuthorizeApis.AnyAsync(u => u.RouterPath.Trim() == routerPath.Trim()))
                            continue;

                        var desc = method.GetCustomAttribute<DescriptionAttribute>()?.Description;
                        apis.Add(new AuthorizeApi
                        {
                            RouterPath = routerPath,
                            Desc = desc ?? string.Empty
                        });
                    }
                }
            }

            await _authorizeApiRepository.BatchAddAsync(apis);
            return new ResponseModel<int> { Code = 200, Msg = "成功", Data = apis.Count() }; // 返回扫描到的 “新” api数目
        }
    }
}
