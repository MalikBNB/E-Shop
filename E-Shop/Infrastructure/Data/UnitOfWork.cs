using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Repositories;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        //private Hashtable _repositories;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();


        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();   
        }

        // .Net 9 implimentation
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => new GenericRepository<TEntity>(_dbContext)
            );
        }

        //public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        //{
        //    if(_repositories is null ) _repositories = new Hashtable();

        //    var type = typeof(TEntity).Name;

        //    if (!_repositories.ContainsKey(type))
        //    {
        //        var repoType = typeof(GenericRepository<>);
        //        var repoInstance = Activator.CreateInstance(repoType.MakeGenericType(typeof(TEntity)), _dbContext);

        //        _repositories.Add(type, repoInstance);
        //    }

        //    return (IGenericRepository<TEntity>)_repositories[type];
        //}
    }

    /*
     Yes — in .NET Core (and now .NET 9 / .NET 8+), while your current implementation is **functional and common**, there are some **modern improvements** you can make to align with:

        * **Strong typing**
        * **Dependency Injection (DI) best practices**
        * **Avoiding reflection when possible**
        * **Cleaner and thread-safe caching**

        Let’s look at how you can **refactor/improve** this implementation.

        ---

        ## ✅ Improved Repository Method using .NET Best Practices

        ### 🔄 Replace `Hashtable` with `ConcurrentDictionary`

        Reflection with `Activator.CreateInstance` is costly and using `Hashtable` is outdated and not type-safe.

        #### 🔁 Before:

        ```csharp
        private Hashtable _repositories;
        ```

        #### ✅ After:

        ```csharp
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        ```

        * `ConcurrentDictionary` is thread-safe and avoids boxing keys into strings.
        * `Type` is more precise and avoids issues with types of the same name in different namespaces.

        ---

        ### 🔧 Modernized Method

        ```csharp
        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return (IGenericRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => new GenericRepository<TEntity>(_dbContext)
            );
        }
        ```

        #### 🚀 Benefits:

        * Uses `GetOrAdd` — atomic operation (thread-safe).
        * No need for reflection.
        * Clean and readable.
        * Strongly typed.

        ---

        ### 🤝 Even Better: Register Generic Repositories in DI

        If your architecture allows it, you can **let .NET Core's built-in DI container create the repositories**, which is **even cleaner**.

        #### 1. Register generic repository in `Startup.cs` or `Program.cs`:

        ```csharp
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        ```

        #### 2. Inject `IGenericRepository<T>` directly:

        You could then use **constructor injection**:

        ```csharp
        public class ProductsService
        {
            private readonly IGenericRepository<Product> _productRepository;

            public ProductsService(IGenericRepository<Product> productRepository)
            {
                _productRepository = productRepository;
            }

            // use _productRepository here...
        }
        ```

        #### OR in Unit of Work, inject `IServiceProvider` (if you still want to keep central Repository method):

        ```csharp
        private readonly IServiceProvider _serviceProvider;

        public UnitOfWork(AppDbContext dbContext, IServiceProvider serviceProvider)
        {
            _dbContext = dbContext;
            _serviceProvider = serviceProvider;
        }

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            return _serviceProvider.GetRequiredService<IGenericRepository<TEntity>>();
        }
        ```

        ---

        ## 🔚 Summary of Improvements

        | Area               | Original                      | Improved                             |
        | ------------------ | ----------------------------- | ------------------------------------ |
        | Caching            | `Hashtable` with `string` key | `ConcurrentDictionary<Type, object>` |
        | Instance creation  | Reflection (`Activator`)      | Direct constructor / DI              |
        | Thread safety      | ❌                             | ✅                                    |
        | Type safety        | ❌                             | ✅                                    |
        | .NET Core practice | Legacy style                  | Modern DI-oriented                   |

        ---

        Would you like me to generate a full working example (with DI, DbContext, UnitOfWork, Repositories) using this cleaner design?

     */
}
