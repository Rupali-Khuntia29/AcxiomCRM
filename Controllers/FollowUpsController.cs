using AcxiomCRM.Data;
using AcxiomCRM.Models;
using AcxiomCRM.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class FollowUpsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;
    public FollowUpsController(ApplicationDbContext db, IAuditService audit) { _db = db; _audit = audit; }
    public async Task<IActionResult> Index() => View(await _db.FollowUps.OrderBy(x => x.FollowUpDate).ToListAsync());
    public IActionResult Create() => View(new FollowUp());
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FollowUp model)
    {
        if (model.FollowUpDate.Date < DateTime.Today)
            ModelState.AddModelError(nameof(model.FollowUpDate), "Follow-up date cannot be earlier than today.");
        if (!ModelState.IsValid) return View(model);
        _db.FollowUps.Add(model); await _db.SaveChangesAsync();
        await _audit.LogAsync("Create", "FollowUp", model.FollowUpId.ToString(), null, model.Subject);
        return RedirectToAction(nameof(Index));
    }
}
