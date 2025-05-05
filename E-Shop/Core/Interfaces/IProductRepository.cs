using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Specifications;

namespace Core.Interfaces
{
    public interface IProductRepository
    {
        Task<Product> GetByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetEntityWithSpec(ISpecification<Product> spec);
        Task<IEnumerable<Product>> ListAsync(ISpecification<Product> spec);
    }
}
