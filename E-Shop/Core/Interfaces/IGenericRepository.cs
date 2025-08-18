
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        Task<IReadOnlyList<T>> GetAllAsync();
        Task<T> GetEntityWithSpec(ISpecification<T> spec);
        Task<IReadOnlyList<T>> ListAsync(ISpecification<T> spec);
        Task<int> CountAsync(ISpecification<T> spec);
        // Why we don't make those methods Async ? => Because they are not responsible to comunicate with Db they just tell EF to track the entity
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        bool Exists(int id);
    }
}
