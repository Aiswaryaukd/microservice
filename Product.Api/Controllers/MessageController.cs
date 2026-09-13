using Microsoft.AspNetCore.Mvc;
using Product.Api.Models;
using Product.Application.Abstract;
using Product.Domain.Entities;

namespace Product.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _service;

        public MessageController(IMessageService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var items = await _service.GetAllAsync();
            var dtos = items.Select(m => new MessageDto { Id = m.Id, Content = m.Content }).ToList();
            return Ok(dtos);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(new MessageDto { Id = item.Id, Content = item.Content });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MessageCreateDto model)
        {
            if (model == null)
                return BadRequest();

            if (!TryValidateModel(model))
                return ValidationProblem(ModelState);

            var domain = new MessageClass { Content = model.Content.Trim() };
            var created = await _service.CreateAsync(domain);

            var dto = new MessageDto { Id = created.Id, Content = created.Content };
            return CreatedAtAction(nameof(Get), new { id = dto.Id }, dto);
        }
    }
}
