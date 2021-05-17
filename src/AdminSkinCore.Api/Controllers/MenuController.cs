using AutoMapper;
using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.Common.CustomAttribute;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.Controllers
{
    /// <summary>
    /// 菜单服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MenuController : ControllerBase
    {
        /// <summary>
        /// 菜单应用服务
        /// </summary>
        private readonly IMenuService _menuService;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="menuService"></param>
        /// <param name="mapper"></param>
        public MenuController(IMenuService menuService, IMapper mapper)
        {
            _menuService = menuService;
            _mapper = mapper;
        }

        /// <summary>
        /// 添加菜单
        /// </summary>
        /// <returns></returns>
        [HttpPost("AddMenu")]
        [Description("添加菜单")]
        public async Task<ResponseModel> AddMenu([FromBody] AddMenuRequest req)
            => await _menuService.AddMenu(_mapper.Map<Menu>(req));

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("删除菜单")]
        [HttpDelete("RemoveMenu/{id}")]
        public async Task<ResponseModel> RemoveMenu(int id)
            => await _menuService.RemoveMenu(id);

        /// <summary>
        /// 获取整颗菜单树
        /// </summary>
        /// <returns></returns>
        [Description("获取整颗菜单树")]
        [HttpGet("GetMenuTree")]
        public async Task<ResponseModel<List<MenuTree>>> GetMenuTree()
            => await _menuService.GetMenuTree();

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [Description("编辑菜单")]
        [HttpPut("EditMenu")]
        public async Task<ResponseModel> EditMenu([FromBody] EditMenuRequest req)
            => await _menuService.EditMenu(_mapper.Map<Menu>(req));

        /// <summary>
        /// 获取属于用户自己的菜单树
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserMenuTree")]
        [DoNotCheckPermission] // 这个接口只要是用户已经登录了就可以调用，不需要权限
        public async Task<ResponseModel<List<MenuTree>>> GetUserMenuTree()
            => await _menuService.GetMenuTree(long.Parse(HttpContext.User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value));
    }
}
