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
    /// 角色应用服务
    /// </summary>
    public interface IRoleService : IService
    {
        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="authorizeApiIds">角色下管辖的api接口记录id数组</param>
        /// <param name="menuIds">菜单id数组</param>
        /// <returns></returns>
        Task<ResponseModel> AddRole(Role role, long[] authorizeApiIds, long[] menuIds);
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">角色id</param>
        /// <returns></returns>
        Task<ResponseModel> RemoveRole(long id);
        /// <summary>
        /// 修改角色
        /// </summary>
        /// <param name="role">角色实体</param>
        /// <param name="authorizeApiIds">角色下管辖的api接口记录id数组</param>
        /// <param name="menuIds">菜单id数组</param>
        /// <returns></returns>
        Task<ResponseModel> EditRole(Role role, long[] authorizeApiIds, long[] menuIds);
        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pagesize">每页显示的数量</param>
        /// <param name="name">角色名(模糊匹配)</param>
        /// <returns></returns>
        Task<ResponseModel<RolePageData>> GetRolePageData(int page, int pagesize, string name);
        /// <summary>
        /// 获取所有角色
        /// </summary>
        /// <returns></returns>
        Task<ResponseModel<List<RoleBasicInfo>>> GetAllRole();
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">角色id数组</param>
        /// <returns></returns>
        Task<ResponseModel> BatchRemove(long[] ids);
    }
}
