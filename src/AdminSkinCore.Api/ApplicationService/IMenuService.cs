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
    /// 菜单应用服务
    /// </summary>
    public interface IMenuService : IService
    {
        /// <summary>
        /// 添加菜单节点
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<ResponseModel> AddMenu(Menu menu);
        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        Task<ResponseModel> RemoveMenu(long id);
        /// <summary>
        /// 获取整颗菜单树
        /// </summary>
        /// <returns></returns>
        Task<ResponseModel<List<MenuTree>>> GetMenuTree();
        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        Task<ResponseModel> EditMenu(Menu menu);
        /// <summary>
        /// 获取用户的菜单树
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<ResponseModel<List<MenuTree>>> GetMenuTree(long id);
    }
}
