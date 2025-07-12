using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Entities;

namespace Core.Specifications
{
    public class ProductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
    {
        public ProductsWithTypesAndBrandsSpecification(ProductSpecParams productParams)
            : base(p => 
                (string.IsNullOrEmpty(productParams.Search) || p.Name.ToLower().Contains(productParams.Search!)) &&
                (string.IsNullOrWhiteSpace(productParams.BrandId) || productParams.BrandId.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Contains(p.ProductBrandId.ToString())) &&
                (string.IsNullOrWhiteSpace(productParams.TypeId) || productParams.TypeId.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList().Contains(p.ProductTypeId.ToString()))
            )
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
            AddOrderBy(x => x.Name);
            ApplyPaging(productParams.PageSize * (productParams.PageIndex - 1),
                productParams.PageSize);

            if (string.IsNullOrEmpty(productParams.Sort)) return;

            switch (productParams.Sort)
            {
                case "priceAsc":
                    AddOrderBy(p => p.Price);
                    break;
                case "priceDesc":
                    AddOrderByDesc(p => p.Price);
                    break;
                default:
                    AddOrderBy(p => p.Name);
                    break;
            }
        }

        public ProductsWithTypesAndBrandsSpecification(int id) 
            : base(p => p.Id == id)
        {
            AddInclude(p => p.ProductType);
            AddInclude(p => p.ProductBrand);
        }
    }
}
