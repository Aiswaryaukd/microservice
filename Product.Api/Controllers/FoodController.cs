using Microsoft.AspNetCore.Mvc;
using Product.Api.Models;
using Product.Application.Abstract;
using Product.Domain.Entities;

namespace Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly IFoodService _service;
        private readonly ILogger<FoodController> _logger;

        public FoodController(IFoodService service, ILogger<FoodController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("GET /api/food started");

            var items = await _service.GetAllAsync();
            _logger.LogInformation("GET /api/food service returned {Count} rows", items.Count);

            var dtos = items.Select(f => new FoodDto { Id = f.Id, Name = f.Name }).ToList();
            _logger.LogInformation("GET /api/food mapped DTOs, returning 200 OK");

            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            _logger.LogInformation("GET /api/food/{Id} started", id);

            var item = await _service.GetByIdAsync(id);
            if (item == null)
            {
                _logger.LogWarning("GET /api/food/{Id} not found", id);
                return NotFound();
            }

            _logger.LogInformation("GET /api/food/{Id} found Name={Name}", id, item.Name);
            return Ok(new FoodDto { Id = item.Id, Name = item.Name });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] FoodCreateDto model)
        {
            _logger.LogInformation("POST /api/food started Name={Name}", model?.Name);

            if (model == null)
            {
                _logger.LogWarning("POST /api/food body is null");
                return BadRequest();
            }

            if (!TryValidateModel(model))
            {
                _logger.LogWarning("POST /api/food validation failed");
                return ValidationProblem(ModelState);
            }

            var created = await _service.CreateAsync(new FoodClass { Name = model.Name.Trim() });
            _logger.LogInformation("POST /api/food saved Id={Id} Name={Name}", created.Id, created.Name);

            var dto = new FoodDto { Id = created.Id, Name = created.Name };
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
    }
}
