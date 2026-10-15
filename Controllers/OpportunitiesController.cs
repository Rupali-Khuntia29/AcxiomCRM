using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class OpportunitiesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    public OpportunitiesController(ApplicationDbContext db, IAuditService audit) { _db = db; _audit = audit; }

    public async Task<IActionResult> Index() => View(await _db.Opportunities.Include(x => x.Customer).OrderByDescending(x => x.CreatedDate).ToListAsync());
    public async Task<IActionResult> Create()
    {
        ViewBag.Customers = await _db.Customers.Where(x => x.Status == "Active").OrderBy(x => x.CustomerName).ToListAsync();
        return View(new Opportunity());
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Opportunity model)
    {
        if (model.Amount <= 0) ModelState.AddModelError(nameof(model.Amount), "Opportunity Amount must be greater than 0.");
        if (model.Probability < 0 || model.Probability > 100) ModelState.AddModelError(nameof(model.Probability), "Probability must be between 0 and 100.");
        if (model.ExpectedCloseDate.Date < DateTime.Today && model.Stage != "Won" && model.Stage != "Lost")
            ModelState.AddModelError(nameof(model.ExpectedCloseDate), "Expected Close Date cannot be in the past.");
        if (!ModelState.IsValid)
        {
            ViewBag.Customers = await _db.Customers.Where(x => x.Status == "Active").ToListAsync();
            return View(model);
        }
        model.Status = model.Stage is "Won" or "Lost" ? model.Stage : "Open";
        _db.Opportunities.Add(model); await _db.SaveChangesAsync();
        await _audit.LogAsync("Create", "Opportunity", model.OpportunityId.ToString(), null, model.OpportunityName);
        return RedirectToAction(nameof(Index));
    }
}
