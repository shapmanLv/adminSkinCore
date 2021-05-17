using AdminSkinCore.Api.EFCoreRepository.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Entities
{
    /// <summary>
    /// 菜单
    /// </summary>
    public class Menu : Entity<long>
    {
        /// <summary>
        /// 上一级菜单
        /// 0 表示一定无父级节点
        /// </summary>
        public long ParentId { get; set; } = 0;
        /// <summary>
        /// 父级菜单id字符串
        /// </summary>
        public string ParentIdStr { get; set; } = "";
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// 前端路由
        /// </summary>
        public string RouterPath { get; set; } = "";
        /// <summary>
        /// 前端路由名称（路由标识）
        /// </summary>
        public string RouterName { get; set; } = "";
        /// <summary>
        /// 描述
        /// </summary>
        public string Desc { get; set; } = "";
        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; } = "";
        /// <summary>
        /// 是否隐藏此菜单
        /// </summary>
        public bool Hidden { get; set; } = false;
        /// <summary>
        /// 排序，排序值越大，同级菜单里，页面菜单中排名越前
        /// </summary>
        public int Sort { get; set; } = 0;
    }
}
