using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using VNet.EntityFramework.Extension.UnitofWork.Abstractions;

namespace VNet.EntityFramework.Extension.UnitofWork
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbContext _dbContext;
        protected readonly DbSet<T> DbSet;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            DbSet = _dbContext.Set<T>();
        }

        #region 查询操作

        /// <summary>
        /// 获取 IQueryable，支持进一步的查询组合
        /// </summary>
        public IQueryable<T> Query()
        {
            return DbSet;
        }

        /// <summary>
        /// 获取不跟踪的 IQueryable
        /// </summary>
        public IQueryable<T> QueryAsNoTracking()
        {
            return DbSet.AsNoTracking();
        }

        /// <summary>
        /// 根据条件查询，返回 IQueryable
        /// </summary>
        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        /// <summary>
        /// 根据条件查询（不跟踪），返回 IQueryable
        /// </summary>
        public IQueryable<T> WhereAsNoTracking(Expression<Func<T, bool>> predicate)
        {
            return DbSet.AsNoTracking().Where(predicate);
        }

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        public async Task<T> GetByIdAsync(object id)
        {
            return await DbSet.FindAsync(id);
        }

        /// <summary>
        /// 根据ID获取实体（同步版本）
        /// </summary>
        public T GetById(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        public async Task<List<T>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        /// <summary>
        /// 获取所有实体（不跟踪）
        /// </summary>
        public async Task<List<T>> GetAllAsNoTrackingAsync()
        {
            return await DbSet.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// 根据条件获取第一个实体
        /// </summary>
        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.FirstOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 根据条件获取第一个实体（同步版本）
        /// </summary>
        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.FirstOrDefault(predicate);
        }

        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        public async Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.SingleOrDefaultAsync(predicate);
        }

        /// <summary>
        /// 根据条件获取单个实体（同步版本）
        /// </summary>
        public T SingleOrDefault(Expression<Func<T, bool>> predicate)
        {
            return DbSet.SingleOrDefault(predicate);
        }

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        /// <summary>
        /// 根据条件获取列表（不跟踪）
        /// </summary>
        public async Task<List<T>> FindAsNoTrackingAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(predicate).ToListAsync();
        }

        #endregion

        #region 统计操作

        /// <summary>
        /// 获取总数量
        /// </summary>
        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? await DbSet.CountAsync()
                : await DbSet.CountAsync(predicate);
        }

        /// <summary>
        /// 获取总数量（同步版本）
        /// </summary>
        public int Count(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? DbSet.Count()
                : DbSet.Count(predicate);
        }

        /// <summary>
        /// 获取长整型总数量
        /// </summary>
        public async Task<long> LongCountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate == null
                ? await DbSet.LongCountAsync()
                : await DbSet.LongCountAsync(predicate);
        }

        /// <summary>
        /// 检查是否存在符合条件的实体
        /// </summary>
        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        /// <summary>
        /// 检查是否存在符合条件的实体（同步版本）
        /// </summary>
        public bool Exists(Expression<Func<T, bool>> predicate)
        {
            return DbSet.Any(predicate);
        }

        #endregion

        #region 分页操作

        /// <summary>
        /// 分页查询
        /// </summary>
        public async Task<PagedResult<T>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool includeTotal = true,
            bool asNoTracking = true)
        {
            var query = asNoTracking ? DbSet.AsNoTracking() : DbSet.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = 0;
            if (includeTotal)
            {
                totalCount = await query.CountAsync();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip(Math.Max(0, pageIndex - 1) * Math.Max(1, pageSize))
                .Take(Math.Max(1, pageSize))
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = includeTotal ? (int)Math.Ceiling((double)totalCount / pageSize) : 0
            };
        }

        /// <summary>
        /// 分页查询（带投影）
        /// </summary>
        public async Task<PagedResult<TResult>> GetPagedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool includeTotal = true)
        {
            var query = DbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = 0;
            if (includeTotal)
            {
                totalCount = await query.CountAsync();
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            var items = await query
                .Skip(Math.Max(0, pageIndex - 1) * Math.Max(1, pageSize))
                .Take(Math.Max(1, pageSize))
                .Select(selector)
                .ToListAsync();

            return new PagedResult<TResult>
            {
                Items = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalPages = includeTotal ? (int)Math.Ceiling((double)totalCount / pageSize) : 0
            };
        }

        #endregion

        #region 添加操作

        /// <summary>
        /// 添加实体
        /// </summary>
        public async Task<T> AddAsync(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entry = await DbSet.AddAsync(entity);
            return entry.Entity;
        }

        /// <summary>
        /// 添加实体（同步版本）
        /// </summary>
        public T Add(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entry = DbSet.Add(entity);
            return entry.Entity;
        }

        /// <summary>
        /// 批量添加实体
        /// </summary>
        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            await DbSet.AddRangeAsync(entities);
        }

        /// <summary>
        /// 批量添加实体（同步版本）
        /// </summary>
        public void AddRange(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            DbSet.AddRange(entities);
        }

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        public T Update(T entity)
        {
            if (entity == null) throw new ArgumentNullException(nameof(entity));

            var entry = DbSet.Update(entity);
            return entry.Entity;
        }

        /// <summary>
        /// 批量更新实体
        /// </summary>
        public void UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null) throw new ArgumentNullException(nameof(entities));

            DbSet.UpdateRange(entities);
        }

        /// <summary>
        /// 部分更新实体
        /// </summary>
        public async Task<T> UpdatePartialAsync(object id, Action<T> updateAction)
        {
            var entity = await GetByIdAsync(id);
            if (entity == null) return null;

            updateAction(entity);
            return entity;
        }

        #endregion

        #region 删除操作

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        public async Task<bool> DeleteAsync(object id)
        {
            var entity = await DbSet.FindAsync(id);
            if (entity == null) return false;

            DbSet.Remove(entity);
            return true;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        public bool Delete(T entity)
        {
            if (entity == null) return false;

            DbSet.Remove(entity);
            return true;
        }

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        public async Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {
            var entities = await DbSet.Where(predicate).ToListAsync();
            if (!entities.Any()) return 0;

            DbSet.RemoveRange(entities);
            return entities.Count;
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        public int DeleteRange(IEnumerable<T> entities)
        {
            if (entities == null) return 0;

            var entityList = entities.ToList();
            if (!entityList.Any()) return 0;

            DbSet.RemoveRange(entityList);
            return entityList.Count;
        }

        #endregion

        #region 兼容性方法（保持向后兼容）

        [Obsolete("Use QueryAsNoTracking() instead")]
        public IQueryable<T> AsNoTracking()
        {
            return QueryAsNoTracking();
        }

        [Obsolete("Use UpdateAsync is not needed, use Update instead")]
        public Task UpdateAsync(T entity)
        {
            Update(entity);
            return Task.CompletedTask;
        }

        [Obsolete("Use UpdateRange instead")]
        public Task UpdateRangeAsync(IEnumerable<T> entities)
        {
            UpdateRange(entities);
            return Task.CompletedTask;
        }

        [Obsolete("Use DeleteRange instead")]
        public Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            DeleteRange(entities);
            return Task.CompletedTask;
        }

        #endregion
    }

    /// <summary>
    /// 分页结果类
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}