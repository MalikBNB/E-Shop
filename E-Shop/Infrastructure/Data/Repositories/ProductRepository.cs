
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        protected readonly AppDbContext _context;
        internal DbSet<Product> dbSet;

        public ProductRepository(AppDbContext context)
        {
            _context = context;
            dbSet = context.Set<Product>();
        }


        public async Task<Product> GetByIdAsync(int id)
        {
            return await dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await dbSet.ToListAsync();
        }

        public async Task<Product> GetEntityWithSpec(ISpecification<Product> spec)
        {
            return await ApplySpecification(spec).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> ListAsync(ISpecification<Product> spec)
        {
            return await ApplySpecification(spec).ToListAsync();
        }


        private IQueryable<Product> ApplySpecification(ISpecification<Product> spec)
        {
            return SpecificationEvaluator<Product>.GetQuery(dbSet.AsQueryable(), spec);
        }
    }
}
