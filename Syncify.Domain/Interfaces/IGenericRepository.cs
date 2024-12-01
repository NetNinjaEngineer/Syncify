using Syncify.Domain.Entities;
using Syncify.Domain.Specifications;

namespace Syncify.Domain.Interfaces;
public interface IGenericRepository<T> where T : BaseEntity
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(Guid id);
    Task<T?> GetBySpecificationAsync(IBaseSpecification<T> specification);
    Task<T?> GetBySpecificationAndIdAsync(IBaseSpecification<T> specification, Guid id);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
}
