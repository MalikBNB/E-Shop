using API.DTOs;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class ProductsController : BaseApiController
    {
        private readonly IGenericRepository<Product> _repo;
        private readonly IGenericRepository<ProductBrand> _productBrandRepo;
        private readonly IGenericRepository<ProductType> _productTypeRepo;
        private readonly IMapper _mapper;

        public ProductsController(IGenericRepository<Product> repository,
            IGenericRepository<ProductBrand> productBrandRepo,
            IGenericRepository<ProductType> productTypeRepo,
            IMapper mapper)
        {
            _repo = repository;
            _productBrandRepo = productBrandRepo;
            _productTypeRepo = productTypeRepo;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(Pagination<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(specParams);
            var countSpec = new ProductsWithFiltersForCountSpecification(specParams);

            var products = await _repo.ListAsync(spec);
            var totalCount = await _repo.CountAsync(countSpec);

            var data = _mapper.Map<IEnumerable<ProductResponseDto>>(products);

            return Ok(new Pagination<ProductResponseDto>(
                        specParams.PageIndex, 
                        specParams.PageSize, 
                        totalCount, 
                        data)
            );
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)] // These two lines are for swaggerUI to inform the client
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]  // about the response he may get if success or not found                                                                                 
        public async Task<IActionResult> GetProduct(int id)                      
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _repo.GetEntityWithSpec(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(_mapper.Map<ProductResponseDto>(product));
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            return Ok(await _productBrandRepo.GetAllAsync());
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTypes()
        {
            return Ok(await _productTypeRepo.GetAllAsync());
        }
    }
}
