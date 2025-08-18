using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Specifications;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        protected async Task<ActionResult> CreatePagedResult<T>(IGenericRepository<T> repo,
        ISpecification<T> spec, ISpecification<T> coutSpec, int pageIndex, int pageSize) where T : BaseEntity
        {
            var items = await repo.ListAsync(spec);
            var count = await repo.CountAsync(coutSpec);

            var pagination = new Pagination<T>(pageIndex, pageSize, count, (IReadOnlyList<T>)items);

            return Ok(pagination);
        }

        protected async Task<ActionResult> CreatePagedResult<T, TDto>(IGenericRepository<T> repo,
        ISpecification<T> spec, ISpecification<T> coutSpec, int pageIndex, int pageSize, IMapper mapper) where T
            : BaseEntity
        {
            var items = await repo.ListAsync(spec);
            var count = await repo.CountAsync(coutSpec);

            var dtoItems = mapper.Map<IEnumerable<TDto>>(items);

            var pagination = new Pagination<TDto>(pageIndex, pageSize, count, (IReadOnlyList<TDto>)dtoItems);

            return Ok(pagination);
        }
    }
}
