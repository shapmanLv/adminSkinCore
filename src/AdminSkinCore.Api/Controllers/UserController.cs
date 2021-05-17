using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.ApplicationService.Impl;
using AdminSkinCore.Api.Common.CustomAttribute;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.Controllers
{
    /// <summary>
    /// 用户服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        /// <summary>
        /// 用户应用服务
        /// </summary>
        private readonly IUserService _userService;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="userService"></param>
        /// <param name="mapper"></param>
        public UserController(IUserService userService,IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("AddUser")]
        [Description("添加用户")]
        [Authorize]
        public async Task<ResponseModel> AddUser([FromBody] AddUserRequest req)
            => await _userService.AddUser(_mapper.Map<User>(req), req.RoleIds);

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("删除用户")]
        [HttpDelete("RemoveUser/{id}")]
        [Authorize]
        public async Task<ResponseModel> RemoveUser(int id)
            => await _userService.RemoveUser(id);

        /// <summary>
        /// 分页获取用户信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("分页获取用户信息")]
        [HttpPost("GetUserPageData")]
        [Authorize]
        public async Task<ResponseModel<UserPageData>> GetUserPageData([FromBody] GetUserPageDataRequest req)
            => await _userService.GetUserPageData(req.Page, req.Pagesize, req.Name, req.Account);

        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        [Description("获取全部用户")]
        [HttpGet("GetAllUser")]
        [Authorize]
        public async Task<ResponseModel<List<UserBasicInfo>>> GetAllUser()
            => await _userService.GetAllUser();

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("EditUser")]
        [Description("编辑用户")]
        [Authorize]
        public async Task<ResponseModel> EditUser([FromBody] EditUserRequest req)
            => await _userService.EditUser(_mapper.Map<User>(req), req.RoleIds);

        /// <summary>
        /// 获取用户个人信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserInfo")]
        [Authorize]
        [DoNotCheckPermission]
        public async Task<ResponseModel<UserBasicInfo>> GetUserInfo()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier);
            if (userId.Value == null)
                return new ResponseModel<UserBasicInfo> { Code = 10001, Msg = "未找到用户信息，请尝试退出登录后重试" };

            var userinfo = await _userService.GetUserInfo(long.Parse(userId.Value));
            if(userinfo == null)
                return new ResponseModel<UserBasicInfo> { Code = 10002, Msg = "未找到用户信息" };
            return new ResponseModel<UserBasicInfo> { Code = 200, Msg = "成功", Data = userinfo };
        }

        /// <summary>
        /// 批量删除用户
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("批量删除用户")]
        [Authorize]
        [HttpDelete("BatchRemoveUser")]
        public async Task<ResponseModel> BatchRemoveUser([FromBody] BatchRemoveUserRequest req)
            => await _userService.BatchRemove(req.UserIds);
    }   
}
