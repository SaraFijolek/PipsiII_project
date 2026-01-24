
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Schematics.API.Data.Entities;
using Schematics.API.Data.Repositories;
using Schematics.API.DTOs.LineCategories;

namespace Schematics.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("line-categories")]
    public class LineCategoryController : ControllerBase
    {
        private readonly ILineCategoryRepository _repository;

        public LineCategoryController(ILineCategoryRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<IList<LineCategoryDto>>> GetAll()
        {
            var categories = await _repository.GetAllAsync();

            var dto = categories.Select(c => new LineCategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                
            }).ToList();

            return Ok(dto);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LineCategoryDto>> GetById(int id)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            return Ok(new LineCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddLinecategory dto)
        {
            var category = new LineCategoryDb
            {
                Name = dto.Name,
                Color = dto.Color
            };

            await _repository.AddAsync(category);
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] EditLineCategory dto)
        {
            var category = await _repository.GetByIdAsync(id);
            if (category == null)
                return NotFound();

            category.Name = dto.Name;
            category.Color = dto.Color;

            await _repository.UpdateAsync(category);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return NoContent();
        }
    }
}
