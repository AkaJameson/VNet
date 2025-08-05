using System.Linq.Expressions;

namespace VNet.EntityFramework.Extension.UnitofWork.Abstractions
{
    public interface IRepository<T> where T : class
    {
        #region 查询操作

        /// <summary>
        /// 获取 IQueryable，支持进一步的查询组合
        /// </summary>
        IQueryable<T> Query();

        /// <summary>
        /// 获取不跟踪的 IQueryable
        /// </summary>
        IQueryable<T> QueryAsNoTracking();

        /// <summary>
        /// 根据条件查询，返回 IQueryable
        /// </summary>
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件查询（不跟踪），返回 IQueryable
        /// </summary>
        IQueryable<T> WhereAsNoTracking(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据ID获取实体
        /// </summary>
        Task<T> GetByIdAsync(object id);

        /// <summary>
        /// 根据ID获取实体（同步版本）
        /// </summary>
        T GetById(object id);

        /// <summary>
        /// 获取所有实体
        /// </summary>
        Task<List<T>> GetAllAsync();

        /// <summary>
        /// 获取所有实体（不跟踪）
        /// </summary>
        Task<List<T>> GetAllAsNoTrackingAsync();

        /// <summary>
        /// 根据条件获取第一个实体
        /// </summary>
        Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取第一个实体（同步版本）
        /// </summary>
        T FirstOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取单个实体
        /// </summary>
        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取单个实体（同步版本）
        /// </summary>
        T SingleOrDefault(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取列表
        /// </summary>
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 根据条件获取列表（不跟踪）
        /// </summary>
        Task<List<T>> FindAsNoTrackingAsync(Expression<Func<T, bool>> predicate);

        #endregion

        #region 统计操作

        /// <summary>
        /// 获取总数量
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取总数量（同步版本）
        /// </summary>
        int Count(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 获取长整型总数量
        /// </summary>
        Task<long> LongCountAsync(Expression<Func<T, bool>> predicate = null);

        /// <summary>
        /// 检查是否存在符合条件的实体
        /// </summary>
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 检查是否存在符合条件的实体（同步版本）
        /// </summary>
        bool Exists(Expression<Func<T, bool>> predicate);

        #endregion

        #region 分页操作

        /// <summary>
        /// 分页查询
        /// </summary>
        Task<PagedResult<T>> GetPagedAsync(
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool includeTotal = true,
            bool asNoTracking = true);

        /// <summary>
        /// 分页查询（带投影）
        /// </summary>
        Task<PagedResult<TResult>> GetPagedAsync<TResult>(
            Expression<Func<T, TResult>> selector,
            int pageIndex,
            int pageSize,
            Expression<Func<T, bool>> predicate = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            bool includeTotal = true);

        #endregion

        #region 添加操作

        /// <summary>
        /// 添加实体
        /// </summary>
        Task<T> AddAsync(T entity);

        /// <summary>
        /// 添加实体（同步版本）
        /// </summary>
        T Add(T entity);

        /// <summary>
        /// 批量添加实体
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// 批量添加实体（同步版本）
        /// </summary>
        void AddRange(IEnumerable<T> entities);

        #endregion

        #region 更新操作

        /// <summary>
        /// 更新实体
        /// </summary>
        T Update(T entity);

        /// <summary>
        /// 批量更新实体
        /// </summary>
        void UpdateRange(IEnumerable<T> entities);

        /// <summary>
        /// 部分更新实体
        /// </summary>
        Task<T> UpdatePartialAsync(object id, Action<T> updateAction);

        #endregion

        #region 删除操作

        /// <summary>
        /// 根据ID删除实体
        /// </summary>
        Task<bool> DeleteAsync(object id);

        /// <summary>
        /// 删除实体
        /// </summary>
        bool Delete(T entity);

        /// <summary>
        /// 根据条件删除实体
        /// </summary>
        Task<int> DeleteAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// 批量删除实体
        /// </summary>
        int DeleteRange(IEnumerable<T> entities);

        #endregion

        #region 兼容性方法（向后兼容）

        [Obsolete("Use QueryAsNoTracking() instead")]
        IQueryable<T> AsNoTracking();

        #endregion
    }
}