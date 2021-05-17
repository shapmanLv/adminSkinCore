using AdminSkinCore.Api.ApplicationService.Base;
using AdminSkinCore.Api.Common;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.ApplicationService
{
    /// <summary>
    /// 用户应用服务
    /// </summary>
    public interface IUserService : IService
    {
        /// <summary>
        /// 添加用户记录
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleIds">角色id数组</param>
        /// <returns></returns>
        Task<ResponseModel> AddUser(User user, long[] roleIds);
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        Task<ResponseModel> RemoveUser(long id);
        /// <summary>
        /// 分页获取用户信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pagesize">每页显示的数量</param>
        /// <param name="name">名字，模糊查询</param>
        /// <param name="account">登录名，模糊查询</param>
        /// <returns></returns>
        Task<ResponseModel<UserPageData>> GetUserPageData(int page, int pagesize, string name, string account);
        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        Task<ResponseModel<List<UserBasicInfo>>> GetAllUser();
        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleIds">角色id数组</param>
        /// <returns></returns>
        Task<ResponseModel> EditUser(User user, long[] roleIds);
        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserBasicInfo> GetUserInfo(long id);
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">用户id数组</param>
        /// <returns></returns>
        Task<ResponseModel> BatchRemove(long[] ids);
    }
}
