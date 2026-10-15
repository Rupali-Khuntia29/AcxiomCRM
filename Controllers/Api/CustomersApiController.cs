using AcxiomCRM.Data;
using AcxiomCRM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers.Api;

[ApiController]
[Route("api/customers")]
[Authorize]
public class CustomersApiController : ControllerBase
{
    private readonly ApplicationDbContext _db;
    public CustomersApiController(ApplicationDbContext db) => _db = db;

    public record CustomerDto(int CustomerId, string CustomerName, string Email, string Phone, string? CompanyName, string Status);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> Get([FromQuery] string? search)
    {
        var q = _db.Customers.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.CustomerName.Contains(search) || x.Email.Contains(search) || (x.CompanyName != null && x.CompanyName.Contains(search)));
        return await q.OrderBy(x => x.CustomerName)
            .Select(x => new CustomerDto(x.CustomerId, x.CustomerName, x.Email, x.Phone, x.CompanyName, x.Status))
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<CustomerDto>> GetById(int id)
    {
        var x = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(x => x.CustomerId == id);
        if (x == null) return NotFound();
        return new CustomerDto(x.CustomerId, x.CustomerName, x.Email, x.Phone, x.CompanyName, x.Status);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Manager,SalesExecutive")]
    public async Task<ActionResult<CustomerDto>> Create([FromBody] Customer input)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);
        if (await _db.Customers.AnyAsync(x => x.Email == input.Email || x.Phone == input.Phone))
            return Conflict(new { message = "A customer with this email or phone already exists." });
        input.CustomerId = 0;
        input.CreatedDate = DateTime.UtcNow;
        _db.Customers.Add(input);
        await _db.SaveChangesAsync();
        var dto = new CustomerDto(input.CustomerId, input.CustomerName, input.Email, input.Phone, input.CompanyName, input.Status);
        return CreatedAtAction(nameof(GetById), new { id = input.CustomerId }, dto);
    }
}
