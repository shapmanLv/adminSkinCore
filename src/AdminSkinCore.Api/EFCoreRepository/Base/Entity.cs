using System;
using System.ComponentModel.DataAnnotations;

namespace AdminSkinCore.Api.EFCoreRepository.Base
{
    /// <summary>
    /// 实体基类
    /// </summary>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class Entity<TKey>
    {
        /// <summary>
        /// 主键id
        /// </summary>
        [Key]
        public TKey Id { get; set; }
        /// <summary>
        /// 记录创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
        /// <summary>
        /// 记录修改时间
        /// </summary>
        public DateTime UpdateTime { get; set; } = DateTime.Now;
    }
}
