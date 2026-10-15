using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize(Roles = "Admin")]
public class AuditController : Controller
{
    private readonly ApplicationDbContext _db;
    public AuditController(ApplicationDbContext db) => _db = db;
    public async Task<IActionResult> Index() =>
        View(await _db.AuditLogs.OrderByDescending(x => x.CreatedDate).Take(500).ToListAsync());
}
