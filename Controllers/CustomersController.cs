using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class CustomersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    public CustomersController(ApplicationDbContext db, IAuditService audit) { _db = db; _audit = audit; }

    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.Customers.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.CustomerName.Contains(search) || c.Email.Contains(search) ||
                (c.Phone != null && c.Phone.Contains(search)) || (c.CompanyName != null && c.CompanyName.Contains(search)));
        ViewBag.Search = search;
        return View(await query.OrderBy(c => c.CustomerName).ToListAsync());
    }

    public IActionResult Create() => View(new Customer());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer model)
    {
        if (model.Email != null && await _db.Customers.AnyAsync(c => c.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "A customer with this email already exists.");
        if (model.Phone != null && await _db.Customers.AnyAsync(c => c.Phone == model.Phone))
            ModelState.AddModelError(nameof(model.Phone), "A customer with this phone number already exists.");
        if (!ModelState.IsValid) return View(model);
        model.CreatedBy = User.Identity?.Name;
        model.CreatedDate = DateTime.UtcNow;
        _db.Customers.Add(model);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Create", "Customer", model.CustomerId.ToString(), null, model.CustomerName);
        TempData["Success"] = "Customer created successfully.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _db.Customers.FindAsync(id);
        return item == null ? NotFound() : View(item);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Customer model)
    {
        if (id != model.CustomerId) return BadRequest();
        if (await _db.Customers.AnyAsync(c => c.CustomerId != id && c.Email == model.Email))
            ModelState.AddModelError(nameof(model.Email), "Email is already used.");
        if (await _db.Customers.AnyAsync(c => c.CustomerId != id && c.Phone == model.Phone))
            ModelState.AddModelError(nameof(model.Phone), "Phone is already used.");
        if (!ModelState.IsValid) return View(model);
        var existing = await _db.Customers.FindAsync(id);
        if (existing == null) return NotFound();
        var oldName = existing.CustomerName;
        existing.CustomerName = model.CustomerName;
        existing.Email = model.Email;
        existing.Phone = model.Phone;
        existing.CompanyName = model.CompanyName;
        existing.Address = model.Address;
        existing.City = model.City;
        existing.State = model.State;
        existing.Status = model.Status;
        existing.ModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Update", "Customer", id.ToString(), oldName, existing.CustomerName);
        TempData["Success"] = "Customer updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin,Manager")]
    public async Task<IActionResult> Deactivate(int id)
    {
        var item = await _db.Customers.FindAsync(id);
        if (item == null) return NotFound();
        item.Status = "Inactive";
        item.ModifiedDate = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        await _audit.LogAsync("Deactivate", "Customer", id.ToString(), "Active", "Inactive");
        return RedirectToAction(nameof(Index));
    }
}
