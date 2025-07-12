using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithFiltersForCountSpecification : BaseSpecification<Product>
    {
        public ProductsWithFiltersForCountSpecification(ProductSpecParams productParams)
            : base(p =>
                (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search!)) &&
                (string.IsNullOrWhiteSpace(productParams.BrandId) || productParams.BrandId.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Contains(p.ProductBrandId.ToString())) &&
                (string.IsNullOrWhiteSpace(productParams.TypeId) || productParams.TypeId.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Contains(p.ProductTypeId.ToString()))
            )
        {

        }
    }
}
