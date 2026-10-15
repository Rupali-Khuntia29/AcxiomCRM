using AcxiomCRM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AcxiomCRM.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _db;
    public DashboardController(ApplicationDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        ViewBag.Customers = await _db.Customers.CountAsync();
        ViewBag.Leads = await _db.Leads.CountAsync();
        ViewBag.OpenLeads = await _db.Leads.CountAsync(x => x.Status != "Lost" && x.Status != "Converted");
        ViewBag.Opportunities = await _db.Opportunities.CountAsync();
        ViewBag.Won = await _db.Opportunities.CountAsync(x => x.Stage == "Won");
        ViewBag.Lost = await _db.Opportunities.CountAsync(x => x.Stage == "Lost");
        ViewBag.Pipeline = await _db.Opportunities.Where(x => x.Status == "Open" && x.Stage != "Won" && x.Stage != "Lost")
            .Select(x => (decimal?)x.Amount).SumAsync() ?? 0m;
        ViewBag.FollowUps = await _db.FollowUps.CountAsync(x => x.Status == "Planned" && x.FollowUpDate >= DateTime.Today);
        ViewBag.RecentCustomers = await _db.Customers.OrderByDescending(x => x.CreatedDate).Take(5).ToListAsync();
        return View();
    }
}
