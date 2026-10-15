using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class LeadsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    public LeadsController(ApplicationDbContext db, IAuditService audit) { _db = db; _audit = audit; }

    public async Task<IActionResult> Index(string? search)
    {
        var q = _db.Leads.AsQueryable();
        if (!string.IsNullOrWhiteSpace(search))
            q = q.Where(x => x.LeadName.Contains(search) || (x.CompanyName != null && x.CompanyName.Contains(search)) || x.Status.Contains(search));
        ViewBag.Search = search;
        return View(await q.OrderByDescending(x => x.CreatedDate).ToListAsync());
    }
    public IActionResult Create() => View(new Lead());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Lead model)
    {
        var allowed = new[] { "New", "Contacted", "Qualified", "Unqualified", "Converted", "Lost" };
        if (!allowed.Contains(model.Status)) ModelState.AddModelError(nameof(model.Status), "Select a valid lead status.");
        if (!ModelState.IsValid) return View(model);
        _db.Leads.Add(model); await _db.SaveChangesAsync();
        await _audit.LogAsync("Create", "Lead", model.LeadId.ToString(), null, model.LeadName);
        return RedirectToAction(nameof(Index));
    }
}
