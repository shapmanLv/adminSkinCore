using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.Configuration.Conventions;
using AdminSkinCore.Api.ApplicationService.Base;
using AdminSkinCore.Api.Dto;
using AdminSkinCore.Api.EFCoreRepository.EFCore;
using AdminSkinCore.Api.EFCoreRepository.Entities;
using AdminSkinCore.Api.EFCoreRepository.Repositories;
using AdminSkinCore.Api.ViewModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdminSkinCore.Api.Common;

namespace AdminSkinCore.Api.ApplicationService.Impl
{
    /// <summary>
    /// 菜单应用服务
    /// </summary>
    public class MenuService : Service, IMenuService
    {
        /// <summary>
        /// 菜单仓储
        /// </summary>
        private readonly IMenuRepository _menuRepository;
        /// <summary>
        /// 角色菜单仓储
        /// </summary>
        private readonly IRoleMenuRepository _roleMenuRepository;
        /// <summary>
        /// automapper
        /// </summary>
        private readonly IMapper _mapper;
        /// <summary>
        /// 配置文件操作对象
        /// </summary>
        private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
        /// <summary>
        /// EF Core上下文
        /// </summary>
        private readonly AdminSkinDbContext _adminSkinDbContext;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="context"></param>
        /// <param name="menuRepository"></param>
        /// <param name="roleMenuRepository"></param>
        /// <param name="mapper"></param>
        /// <param name="configuration"></param>
        /// <param name="adminSkinDbContext"></param>
        public MenuService(AdminSkinDbContext context,
            IMenuRepository menuRepository,
            IRoleMenuRepository roleMenuRepository,
            IMapper mapper,
            Microsoft.Extensions.Configuration.IConfiguration configuration,
            AdminSkinDbContext adminSkinDbContext
            )
        {
            _menuRepository = menuRepository;
            _roleMenuRepository = roleMenuRepository;
            _mapper = mapper;
            _configuration = configuration;
            _adminSkinDbContext = adminSkinDbContext;
        }

        /// <summary>
        /// 添加菜单节点
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public async Task<ResponseModel> AddMenu(Menu menu)
        {
            // 菜单允许同名，但不允许既同名，而且还同级
            if (await _adminSkinDbContext.Menus.Where(u => u.ParentId == menu.ParentId && u.Name == menu.Name).CountAsync() > 0)
                return new ResponseModel { Code = 10001, Msg = "已经存在一个同级且同名的菜单节点，请尝试换个名字，或者更换一个层级" };
            await _menuRepository.AddAsync(menu);
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }

        /// <summary>
        /// 删除菜单
        /// </summary>
        /// <param name="id">菜单id</param>
        /// <returns></returns>
        public async Task<ResponseModel> RemoveMenu(long id)
        {
            using var transaction = await _adminSkinDbContext.Database.BeginTransactionAsync();
            try
            {
                var temp = await _adminSkinDbContext.Menus.FirstOrDefaultAsync(u => u.Id == id);
                if (temp == null)
                    return new ResponseModel { Code = 10001, Msg = "未找到该菜单记录" };

                List<Menu> menuNodes = new List<Menu>();
                menuNodes.Add(temp);
                GetMenuNode(await _adminSkinDbContext.Menus.ToListAsync(), temp.Id, menuNodes);
                await _menuRepository.BatchRemoveAsync(menuNodes);
                List<RoleMenu> roleMenus = new List<RoleMenu>();
                foreach (var item in menuNodes)
                {
                    var roleMenuRecord = await _adminSkinDbContext.RoleMenus.FirstOrDefaultAsync(u => u.MenuId == item.Id);
                    if (roleMenuRecord != null)
                        roleMenus.Add(roleMenuRecord);
                }
                await _roleMenuRepository.BatchRemoveAsync(roleMenus);
                
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
        /// 获取菜单节点
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="parentId">父级菜单</param>
        /// <param name="menuNodes">菜单节点集合</param>
        private void GetMenuNode(List<Menu> dataSource ,long parentId, List<Menu> menuNodes)
        {
            var data = dataSource.Where(u => u.ParentId == parentId);
            if (data == null || data.Count() == 0)
                return;

            foreach (var item in data)
            {
                menuNodes.Add(item);
                GetMenuNode(dataSource, item.Id, menuNodes);
            }
        }

        /// <summary>
        /// 获取整颗菜单树
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseModel<List<MenuTree>>> GetMenuTree()
        {
            var menuData = await _adminSkinDbContext.Menus.ToListAsync(); // 从数据库取出所有菜单数据
            List<MenuTree> menuTrees = new List<MenuTree>(); // 存储最终的菜单树
            CreateTree(menuData, 0, menuTrees); // 前序遍历，添加叶子节点
            return new ResponseModel<List<MenuTree>> { Code = 200, Msg = "成功", Data = menuTrees };
        }

        /// <summary>
        /// 创建菜单树
        /// </summary>
        /// <param name="dataSource">创建菜单树时的数据源</param>
        /// <param name="parentId">父级菜单</param>
        /// <param name="treeNode">菜单树节点</param>
        private void CreateTree(List<Menu> dataSource, long parentId, List<MenuTree> treeNode)
        {
            var data = dataSource.Where(u => u.ParentId == parentId).OrderByDescending(u => u.Sort); // 找同级节点
            if (data != null && data.Count() > 0)
            {
                foreach (var item in data)
                {
                    var node = _mapper.Map<MenuTree>(item);
                    treeNode.Add(node);
                    if (dataSource.Where(u => u.ParentId == node.Id).Count() == 0)
                        continue;
                    node.Children = new List<MenuTree>();
                    CreateTree(dataSource, item.Id, node.Children); // 前序遍历，添加叶子节点
                }
            }
        }

        /// <summary>
        /// 编辑菜单
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public async Task<ResponseModel> EditMenu(Menu menu)
        {
            var record = await _adminSkinDbContext.Menus.FirstOrDefaultAsync(u => u.Id == menu.Id);
            if (record == null)
                return new ResponseModel { Code = 10001, Msg = "此菜单记录不存在，无法修改" };

            record.Name = menu.Name;
            record.ParentId = menu.ParentId;
            record.RouterPath = menu.RouterPath;
            record.Sort = menu.Sort;
            record.Icon = menu.Icon;
            record.RouterName = menu.RouterName;
            record.Hidden = menu.Hidden;
            record.ParentIdStr = menu.ParentIdStr;
            record.Desc = menu.Desc;
            await _adminSkinDbContext.SaveChangesAsync();
            return ResponseModel.BuildResponse(PublicStatusCode.Success);
        }

        /// <summary>
        /// 获取用户的菜单树
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        public async Task<ResponseModel<List<MenuTree>>> GetMenuTree(long id)
        {
            List<MenuTree> menuTrees = new List<MenuTree>();
            var menuRecord = await _adminSkinDbContext.Menus.ToListAsync();
            string adminRoleName = _configuration["Settings:AdminRoleName"];
            var isAdmin = await (from userRole in _adminSkinDbContext.UserRoles.Where(u => u.UserId == id)
                                 join role in _adminSkinDbContext.Roles on userRole.RoleId equals role.Id
                                 where role.Name == adminRoleName
                                 select role.Name).CountAsync() > 0;
            if (isAdmin)
                return await GetMenuTree(); // 如果是管理员，那直接显示所有菜单

            var userMenus = await (from userRole in _adminSkinDbContext.UserRoles.Where(u => u.UserId == id)
                                   join roleMenu in _adminSkinDbContext.RoleMenus on userRole.RoleId equals roleMenu.RoleId
                                   select roleMenu.MenuId).ToListAsync();
            if(userMenus.Count() == 0)
                return new ResponseModel<List<MenuTree>> { Code = 200, Data = menuTrees, Msg = "成功" };

            var nodes = await _adminSkinDbContext.Menus.Where(u => u.ParentId == 0).ToListAsync();
            GetBranchNodeOfTree(menuRecord, userMenus, nodes, menuTrees);
            return new ResponseModel<List<MenuTree>> { Code = 200, Data = menuTrees, Msg = "成功" };
        }

        /// <summary>
        /// 找用户的菜单树分支，构建菜单树
        /// 找的方式有点不同，这个是比较最深的节点，如果最深的节点是符合的，那整个树的分支，就都是用户的菜单树的一部分
        /// </summary>
        /// <param name="dataSource">数据库中的Menu表数据源</param>
        /// <param name="userMenuIds">数据库中和用户关联的菜单记录（数据库中存储的只是零碎的，需要靠这个把这棵树补充完整）</param>
        /// <param name="childrenNodes">子节点</param>
        /// <param name="treeNode">一级一级遍历过程中，菜单树的数据存放点</param>
        /// <returns></returns>
        private bool GetBranchNodeOfTree(List<Menu> dataSource, List<long> userMenuIds, List<Menu> childrenNodes, List<MenuTree> treeNode)
        {
            bool isUserMenuNode = false; // 通过这个实现整一条分叉是否都是用户菜单树的一部分

            foreach (var item in childrenNodes)
            {
                var treeChilrenNodes = dataSource.Where(u => u.ParentId == item.Id).OrderByDescending(u => u.Sort);
                /* 当前节点下面还有子节点 */
                if (treeChilrenNodes != null && treeChilrenNodes.Count() > 0)
                {
                    var temp = new List<MenuTree>();
                    if(GetBranchNodeOfTree(dataSource,userMenuIds,treeChilrenNodes.ToList(),temp))
                    {
                        var node = _mapper.Map<MenuTree>(item);
                        node.Children = temp;
                        treeNode.Add(node);
                        isUserMenuNode = true;
                    }

                    continue;
                }

                /* 当前节点下已经没有子节点 */
                if (!string.IsNullOrEmpty(item.RouterPath.Trim()) && userMenuIds.Where(u => u == item.Id).Count() > 0) // 当前节点必须是有路由才有效
                {
                    var node = _mapper.Map<MenuTree>(item);
                    treeNode.Add(node);
                    isUserMenuNode = true;
                }                
            }

            return isUserMenuNode;
        }
    }
}
