using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoenixContact.Core.Model;
using PhoenixContact.EF;
using PhoenixContact.EF.Model;

namespace PhoenixContact.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly ILogger<EmployeesController> _logger;
        private readonly PhoenixContactDb _context;

        public EmployeesController(ILogger<EmployeesController> logger, PhoenixContactDb context)
        {
            _logger = logger;
            _context = context;
        }

        private Employee MapDtoToEntity(EmployeeDto dto)
        {
            return new Employee
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Salary = dto.Salary,
                PositionLevel = dto.PositionLevel,
                Residence = dto.Residence
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetAll()
        {
            try
            {
                var employees = await _context.Employees.ToListAsync();
                var employeeDtos = employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary,
                    PositionLevel = e.PositionLevel,
                    Residence = e.Residence
                }).ToList();

                return Ok(employeeDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController - GetAll() -  B³¹d podczas pobierania listy pracowników.");
                return StatusCode(500, "Wyst¹pi³ b³¹d serwera podczas pobierania danych.");
            }
        }

        [HttpPost("import")]
        public async Task<IActionResult> ImportEmployees([FromBody] List<EmployeeDto> employees)
        {
            if (employees == null || !employees.Any())
                return BadRequest("Brak danych do zapisania.");

            try
            {
                var entities = employees.Select(MapDtoToEntity).ToList();

                _context.Employees.AddRange(entities);
                await _context.SaveChangesAsync();

                return Ok(new { Message = $"{entities.Count} rekordów zapisano." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController - ImportEmployees - B³¹d podczas importowania pracowników.");
                return StatusCode(500, "Wyst¹pi³ b³¹d serwera podczas zapisywania danych.");
            }

        }

        [HttpGet("top-earners-by-position")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetTopEarnersByPosition()
        {
            try
            {
                var rawResults = await _context.Employees
                    .FromSqlRaw("EXEC [dbo].[GetTopEarnersByPosition]")
                    .AsNoTracking()
                    .ToListAsync();

                var result = rawResults.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary,
                    PositionLevel = e.PositionLevel,
                    Residence = e.Residence
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController - GetTopEarnersByPosition - B³¹d podczas pobierania top earners by position.");
                return StatusCode(500, "Wyst¹pi³ b³¹d serwera podczas pobierania danych.");
            }
        }

        [HttpGet("lowest-earners-by-city")]
        public async Task<ActionResult<IEnumerable<EmployeeDto>>> GetLowestEarnersByCity()
        {
            try
            {
                var rawResults = await _context.Employees
                    .FromSqlRaw("EXEC [dbo].[GetLowestGrossEarnersByCity]")
                    .AsNoTracking()
                    .ToListAsync();

                var result = rawResults.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    Salary = e.Salary,
                    PositionLevel = e.PositionLevel,
                    Residence = e.Residence
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmployeeController - GetLowestEarnersByCity -  B³¹d podczas pobierania lowest earners by city.");
                return StatusCode(500, "Wyst¹pi³ b³¹d serwera podczas pobierania danych.");
            }
        }
    }
}
