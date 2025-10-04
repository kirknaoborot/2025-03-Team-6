using ChannelSettings.Domain.Interfaces;

namespace ChannelSettings.Core.IRepositories
{
    public interface IRepository<T, TPrimaryKey> where T : class, IEntity<TPrimaryKey>
    {
        T Add(T entity);
        Task<T> AddAsync(T entity);
        void AddRange(List<T> entities);
        Task AddRangeAsync(ICollection<T> entities);
        bool Delete(T entity);
        bool Delete(TPrimaryKey id);
        bool DeleteRange(ICollection<T> entities);
        T Get(TPrimaryKey id);
        IQueryable<T> GetAll(bool asNoTracking = false);
        Task<List<T>> GetAllAsync(CancellationToken cancellationToken, bool asNoTracking = false);
        Task<T> GetAsync(TPrimaryKey id, CancellationToken cancellationToken);
        Task<List<T>> GetPagedAsync(int page, int itemsPerPage);
        void SaveChanges();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
        void Update(T entity);
    }
}