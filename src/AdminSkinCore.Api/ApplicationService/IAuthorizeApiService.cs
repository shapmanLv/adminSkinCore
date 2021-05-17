using AdminSkinCore.Api.ApplicationService.Base;
using AdminSkinCore.Api.Common;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ApplicationService
{
    /// <summary>
    /// 需授权的接口服务
    /// </summary>
    public interface IAuthorizeApiService : IService
    {
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        Task<ResponseModel> AddAuthorizeApi(AuthorizeApi api);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">接口记录的id</param>
        /// <returns></returns>
        Task<ResponseModel> RemoveAuthorizeApi(long id);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="api"></param>
        /// <returns></returns>
        Task<ResponseModel> EditAuthorizeApi(AuthorizeApi api);
        /// <summary>
        /// 分页获取需授权的api接口数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pagesize">面页显示的数量</param>
        /// <param name="routerPath">数据筛选条件：接口路径</param>
        /// <returns></returns>
        Task<ResponseModel<AuthorizeApiPageData>> GetAuthorizeApiPageData(int page, int pagesize, string routerPath);
        /// <summary>
        /// 查询所有需授权的api接口
        /// </summary>
        /// <returns></returns>
        Task<ResponseModel<List<AuthorizeApiInfo>>> GetAllAuthorizeApi();
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<ResponseModel> BatchRemove(long[] ids);
        /// <summary>
        /// 扫描所有action，生成AuthorizeApi记录
        /// </summary>
        /// <returns></returns>
        Task<ResponseModel<int>> BuildAuthorizeApis();
    }
}
