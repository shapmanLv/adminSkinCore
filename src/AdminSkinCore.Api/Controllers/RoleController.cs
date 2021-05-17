    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.Controllers
{
    /// <summary>
    /// 角色服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoleController : ControllerBase
    {
        /// <summary>
        /// 角色应用服务
        /// </summary>
        private readonly IRoleService _roleService;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="roleService"></param>
        /// <param name="mapper"></param>
        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>
        /// 添加角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("添加角色")]
        [HttpPost("AddRole")]
        public async Task<ResponseModel> AddRole([FromBody] AddRoleRequest req)
            => await _roleService.AddRole(_mapper.Map<Role>(req), req.authorizeApiIdIds, req.menuIds);

        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("删除角色")]
        [HttpDelete("RemoveRole/{id}")]
        public async Task<ResponseModel> RemoveRole(int id)
            => await _roleService.RemoveRole(id);

        /// <summary>
        /// 编辑角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("编辑角色")]
        [HttpPut("EditRole")]
        public async Task<ResponseModel> EditRole([FromBody] EditRoleRequest req)
            => await _roleService.EditRole(_mapper.Map<Role>(req), req.authorizeApiIdIds, req.menuIds);

        /// <summary>
        /// 分页获取角色信息
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("分页获取角色信息")]
        [HttpPost("GetRolePageData")]
        public async Task<ResponseModel<RolePageData>> GetRolePageData([FromBody] GetRolePageDataRequest req)
            => await _roleService.GetRolePageData(req.Page, req.Pagesize, req.Name);

        /// <summary>
        /// 获取全部角色
        /// </summary>
        /// <returns></returns>
        [Description("获取全部角色")]
        [HttpGet("GetAllRole")]
        public async Task<ResponseModel<List<RoleBasicInfo>>> GetAllRole()
            => await _roleService.GetAllRole();

        /// <summary>
        /// 批量删除角色
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("批量删除角色")]
        [HttpDelete("BatchRemoveRole")]
        public async Task<ResponseModel> BatchRemoveRole([FromBody] BatchRemoveRoleRequest req)
            => await _roleService.BatchRemove(req.RoleIds);
    }
}
