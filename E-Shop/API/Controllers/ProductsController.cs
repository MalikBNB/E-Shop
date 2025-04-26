using API.DTOs;
using API.Errors;
using AutoMapper;
using Core.Entities;
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
        private readonly GenericRepository<Product> _repo;
        private readonly IMapper _mapper;

        public ProductsController(GenericRepository<Product> repository, IMapper mapper)
        {
            _repo = repository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var spec = new ProductsWithTypesAndBrandsSpecification();

            var products = await _repo.ListAsync(spec);

            return Ok(_mapper.Map<List<ProductResponseDto>>(products));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProduct(int id)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await _repo.GetEntityWithSpec(spec);

            return Ok(_mapper.Map<ProductResponseDto>(product));
        }
    }
}
