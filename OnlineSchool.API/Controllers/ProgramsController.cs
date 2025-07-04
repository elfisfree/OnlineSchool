using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineSchool.API.Data;
using OnlineSchool.Shared.DTOs;
using OnlineSchool.Shared.Models;

namespace OnlineSchool.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // Защищаем весь контроллер, доступ только для Admin
    public class ProgramsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProgramsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/programs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProgramDto>>> GetPrograms()
        {
            var programs = await _context.EducationalPrograms
                .Select(p => new ProgramDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToListAsync();

            return Ok(programs);
        }

        // GET: api/programs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ProgramDto>> GetProgram(int id)
        {
            var program = await _context.EducationalPrograms.FindAsync(id);

            if (program == null)
            {
                return NotFound();
            }

            var programDto = new ProgramDto
            {
                Id = program.Id,
                Name = program.Name,
                Description = program.Description
            };

            return Ok(programDto);
        }

        // POST: api/programs
        [HttpPost]
        public async Task<ActionResult<ProgramDto>> CreateProgram([FromBody] ProgramDto programDto)
        {
            var program = new EducationalProgram
            {
                Name = programDto.Name,
                Description = programDto.Description
            };

            _context.EducationalPrograms.Add(program);
            await _context.SaveChangesAsync();

            programDto.Id = program.Id; // Возвращаем DTO с новым ID

            return CreatedAtAction(nameof(GetProgram), new { id = program.Id }, programDto);
        }

        // PUT: api/programs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgram(int id, [FromBody] ProgramDto programDto)
        {
            if (id != programDto.Id)
            {
                return BadRequest();
            }

            var program = await _context.EducationalPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound();
            }

            program.Name = programDto.Name;
            program.Description = programDto.Description;

            _context.Entry(program).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.EducationalPrograms.Any(e => e.Id == id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); // Успешное обновление, нет содержимого для возврата
        }

        // DELETE: api/programs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var program = await _context.EducationalPrograms.FindAsync(id);
            if (program == null)
            {
                return NotFound();
            }

            _context.EducationalPrograms.Remove(program);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}