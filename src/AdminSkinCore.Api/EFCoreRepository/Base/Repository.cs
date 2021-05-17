using AdminSkinCore.Api.EFCoreRepository.EFCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace AdminSkinCore.Api.EFCoreRepository.Base
{
    /// <summary>
    /// 仓储基类
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    /// <typeparam name="TKey">主键类型</typeparam>
    public class Repository<TDbContext, TEntity, TKey> where TEntity : Entity<TKey> where TDbContext : DbContext
    {
        /// <summary>
        /// EF Core上下文
        /// </summary>
        public virtual TDbContext Context { get; private set; }
        protected DbSet<TEntity> _dbSet;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="context"></param>
        public Repository(TDbContext context)
        {
            Context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// 分页获取数据
        /// </summary>
        /// <param name="page">页码</param>
        /// <param name="limit">每页显示数</param>
        /// <param name="total">总记录数</param>
        /// <param name="filterExpression">数据筛选表达式</param>
        /// <param name="orderByExpression">排序表达式</param>
        /// <param name="selectExpression"></param>
        /// <param name="isAsc">是否是正序排序</param>
        /// <returns></returns>
        public virtual IQueryable<TResult> GetPageData<TResult, TOrderKey>(
            int page, int limit, out int total, Expression<Func<TEntity, bool>> filterExpression,
            Expression<Func<TEntity, TOrderKey>> orderByExpression,
            Expression<Func<TEntity, TResult>> selectExpression, bool isAsc)
        {
            total = _dbSet.Where(filterExpression).Count();

            if (isAsc)
            {
                return _dbSet.AsNoTracking().Where(filterExpression).OrderBy(orderByExpression).Skip((page - 1) * limit).Take(limit).Select(selectExpression);
            }

            return _dbSet.AsNoTracking().Where(filterExpression).OrderByDescending(orderByExpression).Skip((page - 1) * limit).Take(limit).Select(selectExpression);
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual async Task AddAsync(TEntity entity)
        {
            _dbSet.Add(entity);
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task BatchAddAsync(IEnumerable<TEntity> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task BatchAddAsync(TEntity[] entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(TEntity entity)
        {
            Context.Entry(entity).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        public virtual async Task RemoveAsync(TKey id)
        {
            var temp = await _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Id.Equals(id));
            if (temp != null)
            {
                Context.Entry(temp).State = EntityState.Deleted;
            }
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual async Task BatchRemoveAsync(IEnumerable<TEntity> entities)
        {
            foreach (var item in entities)
            {
                Context.Entry(item).State = EntityState.Deleted;
            }
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public virtual async Task BatchRemoveAsync(TKey[] ids)
        {
            foreach (var id in ids)
            {
                var temp = await _dbSet.AsNoTracking().FirstOrDefaultAsync(u => u.Id.Equals(id));
                if (temp == null)
                    continue;
                Context.Entry(temp).State = EntityState.Deleted;
            }
            await Context.SaveChangesAsync();
        }
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="filterExpression">数据筛序 表达式树</param>
        /// <returns></returns>
        public virtual async Task BatchRemoveAsync(Expression<Func<TEntity, bool>> filterExpression)
            => await BatchRemoveAsync(await _dbSet.AsNoTracking().Where(filterExpression).ToListAsync());
    }
}
