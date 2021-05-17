using AutoMapper;
using AdminSkinCore.Api.ApplicationService.Base;
using AdminSkinCore.Api.Common.Helper.ExpressionExtend;
using AdminSkinCore.Api.Common.Sms;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.EFCoreRepository.Repositories;
using AdminSkinCore.Api.ViewModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.ApplicationService.Impl
{
    /// <summary>
    /// 用户应用服务
    /// </summary>
    public class UserService : Service, IUserService
    {
        internal class CachePrefix
        {
            /// <summary>
            /// 登录错误
            /// </summary>
            public const string LoginFail = "User:Login:LoginFail";
            /// <summary>
            /// 验证码
            /// </summary>
            public const string Captcha = "User:Login:Captcha";
            /// <summary>
            /// 账户信息，账户禁用状态、登录状态等
            /// </summary>
            public const string AccountInfo = "User:Login:AccountInfo";
            /// <summary>
            /// 接口资源
            /// </summary>
            public const string ApiResource = "User:Permission:ApiResource";
        }

        /// <summary>
        /// 用户仓储
        /// </summary>
        private readonly IUserRepository _userRepository;
        /// <summary>
        /// sha 加密
        /// </summary>
        private readonly IBCryptService _bCryptService;
        /// <summary>
        /// 缓存
        /// </summary>
        private readonly IDistributedCache _cache;
        /// <summary>
        /// 配置文件操作对象
        /// </summary>
        private readonly IConfiguration _configuration;
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger _log;
        /// <summary>
        /// 请求上下文
        /// </summary>
        private readonly HttpContext _httpContext;
        /// <summary>
        /// 用户与角色仓储
        /// </summary>
        private readonly IUserRoleRepository _userRoleRepository;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 机器环境
        /// </summary>
        private readonly IWebHostEnvironment _env;
        /// <summary>
        /// 发送短信
        /// </summary>
        private readonly ISmsSender _smsSender;
        /// <summary>
        /// EF Core上下文
        /// </summary>
        private readonly AdminSkinDbContext _adminSkinDbContext;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="userRepository"></param>
        /// <param name="customBCrypt"></param>
        /// <param name="cache"></param>
        /// <param name="configuration"></param>
        /// <param name="log"></param>
        /// <param name="httpContextAccessor"></param>
        /// <param name="adminSkinDbContext"></param>
        /// <param name="context"></param>
        /// <param name="userRoleRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="env"></param>
        /// <param name="smsSender"></param>
        public UserService(
            IUserRepository userRepository,
            IBCryptService customBCrypt,
            IDistributedCache cache,
            IConfiguration configuration,
            ILogger<UserService> log,
            IHttpContextAccessor httpContextAccessor,
            AdminSkinDbContext adminSkinDbContext,
            IUserRoleRepository userRoleRepository,
            IMapper mapper,
            IWebHostEnvironment env,
            ISmsSender smsSender
           )
        {
            _userRepository = userRepository;
            _bCryptService = customBCrypt;
            _cache = cache;
            _configuration = configuration;
            _log = log;
            _httpContext = httpContextAccessor.HttpContext;
            _adminSkinDbContext = adminSkinDbContext;
            _userRoleRepository = userRoleRepository;
            _mapper = mapper;
            _env = env;
            _smsSender = smsSender;
        }      

        /// <summary>
        /// 添加用户记录
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleIds">角色id数组</param>
        /// <returns></returns>
        public async Task<ResponseModel> AddUser(User user, long[] roleIds)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {
                if (await _adminSkinDbContext.Users.Where(u => u.Account == user.Account).CountAsync() > 0)
                    return new ResponseModel() { Code = 10001, Msg = "此用户名已被注册" };

                user.Password = _bCryptService.HashPassword(user.Password);

                await _userRepository.AddAsync(user);
                List<UserRole> userRoles = new List<UserRole>();
                foreach (var roleId in roleIds)
                {
                    if (await _adminSkinDbContext.Roles.Where(u => u.Id == roleId).CountAsync() > 0)
                        userRoles.Add(new UserRole { RoleId = roleId, UserId = user.Id });
                }
                await _userRoleRepository.BatchAddAsync(userRoles);
                await transaction.CommitAsync();
                return ResponseModel.BuildResponse(PublicStatusCode.Success);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }       

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public async Task<ResponseModel> RemoveUser(long id)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {

                await _userRepository.RemoveAsync(id);
                await _userRoleRepository.BatchRemoveAsync(u => u.UserId == id);
                await transaction.CommitAsync();
                return ResponseModel.BuildResponse(PublicStatusCode.Success);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        /// <summary>
        /// 分页获取用户信息
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="pagesize">每页显示的数量</param>
        /// <param name="name">名字，模糊查询</param>
        /// <param name="account">登录名，模糊查询</param>
        /// <returns></returns>
        public async Task<ResponseModel<UserPageData>> GetUserPageData(int page, int pagesize, string name, string account)
        {
            Expression<Func<User, bool>> expression = u => true;
            if (!string.IsNullOrEmpty(name.Trim()))
                expression = expression.ExpressionAnd(u => u.Name.Contains(name.Trim()));
            if (!string.IsNullOrEmpty(account.Trim()))
                expression = expression.ExpressionAnd(u => u.Account.Contains(account.Trim()));
            var userData = await _userRepository.GetPageData(page, pagesize, out int total, expression, u => u.Id,
                u => new UserInfo()
                {
                    Id = u.Id,
                    Name = u.Name,
                    Account = u.Account,
                    Email = u.Email,
                    LastLoginIp = u.LastLoginIp,
                    LastLoginTime = u.LastLoginTime,
                    PhoneNumber = u.PhoneNumber
                }, false).ToListAsync();

            foreach (var item in userData)
            {
                item.RoleInfos = await (from userRole in _adminSkinDbContext.UserRoles.Where(u => u.UserId == item.Id)
                                        join role in _adminSkinDbContext.Roles on userRole.RoleId equals role.Id
                                        select new RoleBasicInfo
                                        {
                                            Id = role.Id,
                                            Name = role.Name,
                                            Desc = role.Desc
                                        }).ToListAsync();

            }
            return new ResponseModel<UserPageData>
            {
                Data = new UserPageData { UserInfos = userData, TotalCount = total },
                Code = 200,
                Msg = "成功"
            };
        }

        /// <summary>
        /// 获取所有用户
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<UserBasicInfo>>> GetAllUser()
        {
            return new ResponseModel<List<UserBasicInfo>> { Code = 200, Data = _mapper.Map<List<UserBasicInfo>>(await _adminSkinDbContext.Users.ToListAsync()), Msg = "成功" };
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <param name="user">用户实体</param>
        /// <param name="roleIds">角色id数组</param>
        /// <returns></returns>
        public async Task<ResponseModel> EditUser(User user, long[] roleIds)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {
                var userRecord = await _adminSkinDbContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (userRecord == null)
                    return new ResponseModel { Code = 10001, Msg = "此记录已不存在，无法修改" };
                //if (user.Account.Trim() != userRecord.Account && await _adminSkinDbContext.Users.Where(u => u.Account == user.Account).CountAsync() > 0)
                //    return new ResponseModel { Code = 10002, Msg = "此登录名已被注册" };
                if (!string.IsNullOrEmpty(user.Password.Trim())) // 修改密码
                    userRecord.Password = _bCryptService.HashPassword(user.Password);

                userRecord.Name = user.Name;
                userRecord.PhoneNumber = user.PhoneNumber;
                userRecord.Email = user.Email;

                var oldData = await _adminSkinDbContext.UserRoles.Where(u => u.UserId == user.Id).ToListAsync();
                List<UserRole> hasRemoveData = new List<UserRole>(); // 需要删除的数据
                List<UserRole> hasAddData = new List<UserRole>(); // 需要添加到数据库的数据
                bool flag = false; // 循环过程中的标记
                foreach (var roleId in roleIds)
                {
                    foreach (var record in oldData)
                    {
                        if (roleId == record.RoleId)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag && await _adminSkinDbContext.Roles.Where(u => u.Id == roleId).CountAsync() > 0)
                        hasAddData.Add(new UserRole { RoleId = roleId, UserId = user.Id });

                    flag = false;
                }

                foreach (var record in oldData)
                {
                    foreach (var roleId in roleIds)
                    {
                        if (roleId == record.RoleId)
                        {
                            flag = true;
                            break;
                        }
                    }
                    if (!flag)
                        hasRemoveData.Add(record);

                    flag = false;
                }

                await _userRoleRepository.BatchAddAsync(hasAddData);
                await _userRoleRepository.BatchRemoveAsync(hasRemoveData);

                if (hasAddData.Count > 0 || hasRemoveData.Count > 0)
                {
                    userRecord.IsLogin = false; // 用户的角色信息发生变更，强制此用户下线重新登录                    
                    await _adminSkinDbContext.SaveChangesAsync(); // 必须持久化的数据修改成功了，再清除缓存
                    await _cache.RemoveAsync($"{CachePrefix.AccountInfo}:{userRecord.Account.Trim()}"); // 清除用户信息缓存
                }
                else
                    await _adminSkinDbContext.SaveChangesAsync();

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
        /// 获取用户个人信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<UserBasicInfo> GetUserInfo(long id)
            => _mapper.Map<UserBasicInfo>(await _adminSkinDbContext.Users.FirstOrDefaultAsync(u => u.Id == id));

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids">用户id数组</param>
        /// <returns></returns>
        public async Task<ResponseModel> BatchRemove(long[] ids)
        {
            using (var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    List<User> users = new List<User>();
                    List<UserRole> userRoles = new List<UserRole>();

                    foreach (var item in ids)
                    {
                        var userRecord = await _adminSkinDbContext.Users.FirstOrDefaultAsync(u => u.Id == item);
                        if (userRecord != null)
                            users.Add(userRecord);

                        var userRoleRecord = await _adminSkinDbContext.UserRoles.FirstOrDefaultAsync(u => u.UserId == item);
                        if (userRoleRecord != null)
                            userRoles.Add(userRoleRecord);
                    }

                    await _userRepository.BatchRemoveAsync(users);
                    await _userRoleRepository.BatchRemoveAsync(userRoles);

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
    }
}