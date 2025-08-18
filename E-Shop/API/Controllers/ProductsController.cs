using API.DTOs;
using API.Errors;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class ProductsController(IUnitOfWork unitOfWork, IMapper mapper) : BaseApiController
    {
        [HttpGet]
        [ProducesResponseType(typeof(Pagination<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllProducts([FromQuery] ProductSpecParams specParams)
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(specParams);
            var countSpec = new ProductsWithFiltersForCountSpecification(specParams);

            return await CreatePagedResult<Product, ProductResponseDto>(unitOfWork.Repository<Product>(), 
                                           spec, 
                                           countSpec, 
                                           specParams.PageIndex,
                                           specParams.PageSize,
                                           mapper);

        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)] // These two lines are for swaggerUI to inform the client
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]  // about the response he may get if success or not found                                                                                 
        public async Task<IActionResult> GetProduct(int id)                      
        {
            var spec = new ProductsWithTypesAndBrandsSpecification(id);

            var product = await unitOfWork.Repository<Product>().GetEntityWithSpec(spec);

            if (product is null)
                return NotFound(new ApiResponse(404));

            return Ok(mapper.Map<ProductResponseDto>(product));
        }

        [HttpGet("brands")]
        public async Task<IActionResult> GetBrands()
        {
            return Ok(await unitOfWork.Repository<ProductBrand>().GetAllAsync());
        }

        [HttpGet("types")]
        public async Task<IActionResult> GetTypes()
        {
            return Ok(await unitOfWork.Repository<ProductType>().GetAllAsync());
        }

        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            unitOfWork.Repository<Product>().Add(product);

            if (await unitOfWork.CompleteAsync())
            {
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }

            return BadRequest("Problem creating product");
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> UpdateProduct(int id, Product product)
        {
            if (product.Id != id || !ProductExists(id))
                return BadRequest("Cannot update this product");

            unitOfWork.Repository<Product>().Update(product);

            if (await unitOfWork.CompleteAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem updating the product");
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await unitOfWork.Repository<Product>().GetByIdAsync(id);

            if (product == null) return NotFound();

            unitOfWork.Repository<Product>().Delete(product);

            if (await unitOfWork.CompleteAsync())
            {
                return NoContent();
            }

            return BadRequest("Problem deleting the product");
        }

        private bool ProductExists(int id)
        {
            return unitOfWork.Repository<Product>().Exists(id);
        }
    }
}
