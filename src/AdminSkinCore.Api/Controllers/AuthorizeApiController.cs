using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AdminSkinCore.Api.ApplicationService;
using AdminSkinCore.Api.Common.CustomAttribute;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.Controllers
{
    /// <summary>
    /// 需授权的api接口服务
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuthorizeApiController : ControllerBase
    {
        /// <summary>
        /// 需授权的api接口 应用服务
        /// </summary>
        private readonly IAuthorizeApiService _authorizeApiService;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// EF 上下文
        /// </summary>
        private readonly AdminSkinDbContext _context;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="apiService"></param>
        /// <param name="mapper"></param>
        /// <param name="context"></param>
        public AuthorizeApiController(IAuthorizeApiService apiService, IMapper mapper, AdminSkinDbContext context)
        {
            _authorizeApiService = apiService;
            _mapper = mapper;
            _context = context;
        }

        /// <summary>
        /// 添加接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPost("AddAuthorizeApi")]
        [Description("添加接口")]
        public async Task<ResponseModel> AddAuthorizeApi([FromBody] AddAuthorizeApiRequest req)
            => await _authorizeApiService.AddAuthorizeApi(_mapper.Map<AuthorizeApi>(req));

        /// <summary>
        /// 删除接口
        /// </summary>
        /// <param name="id">接口记录id</param>
        /// <returns></returns>
        [HttpDelete("RemoveAuthorizeApi/{id}")]
        [Description("删除接口")]
        public async Task<ResponseModel> RemoveAuthorizeApi(long id)
            => await _authorizeApiService.RemoveAuthorizeApi(id);

        /// <summary>
        /// 修改接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpPut("EditAuthorizeApi")]
        [Description("修改接口")]
        public async Task<ResponseModel> EditAuthorizeApi([FromBody] EditAuthorizeApiRequest req)
            => await _authorizeApiService.EditAuthorizeApi(_mapper.Map<AuthorizeApi>(req));

        /// <summary>
        /// 分页获取需授权的api接口数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("GetAuthorizeApiPageData")]
        [Description("分页获取需授权的api接口数据")]
        public async Task<ResponseModel<AuthorizeApiPageData>> GetAuthorizeApiPageData([FromBody] GetAuthorizeApiPageDataRequest req)
        {
            if (req.RouterPath.Trim() == "")
                return await _authorizeApiService.GetAuthorizeApiPageData(req.Page, req.Pagesize, req.RouterPath);
            else
            {
                string[] temp = req.RouterPath.Split('-');
                return await _authorizeApiService.GetAuthorizeApiPageData(req.Page, req.Pagesize, string.Join('/',temp));
            }
        }

        /// <summary>
        /// 获取所有需授权的api接口数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllAuthorizeApi")]
        [Description("获取所有需授权的api接口数据")]
        public async Task<ResponseModel<List<AuthorizeApiInfo>>> GetAllAuthorizeApi()
             => await _authorizeApiService.GetAllAuthorizeApi();

        /// <summary>
        /// 批量删除需授权的接口
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpDelete("BatchRemoveAuthorizeApi")]
        [Description("批量删除需授权的接口")]
        public async Task<ResponseModel> BatchRemoveAuthorizeApi([FromBody] BatchRemoveAuthorizeApiRequest req)
            => await _authorizeApiService.BatchRemove(req.AuthorizeApiIds);

        /// <summary>
        /// 遍历控制器，将需授权使用的Api记录到数据库表中
        /// </summary>
        /// <returns></returns>
        [HttpPost("BuildAuthorizeApis")]
        [DoNotCheckPermission]
        public async Task<ResponseModel<int>> BuildAuthorizeApis()
            => await _authorizeApiService.BuildAuthorizeApis();
    }
}
