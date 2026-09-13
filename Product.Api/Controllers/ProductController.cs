using Microsoft.AspNetCore.Mvc;
using Product.Application.Abstract;
using Product.Domain.Entities;
using Product.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _service;

        public ProductController(IProductService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _service.GetAllAsync();
            var dtos = items.Select(p => new ProductDto { Id = p.Id, Name = p.Name, Price = p.Price }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            var dto = new ProductDto { Id = item.Id, Name = item.Name, Price = item.Price };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto product)
        {
            if (product == null)
                return BadRequest();

            if (!TryValidateModel(product))
                return ValidationProblem(ModelState);

            var domain = new ProductClass { Name = product.Name, Price = product.Price };
            var created = await _service.CreateAsync(domain);

            var dto = new ProductDto { Id = created.Id, Name = created.Name, Price = created.Price };
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
    }

}
