using AcxiomCRM.Data;
using AcxiomCRM.Models;
using System.Security.Claims;

namespace AcxiomCRM.Services;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _http;
    public AuditService(ApplicationDbContext db, IHttpContextAccessor http)
    {
        _db = db;
        _http = http;
    }

    public async Task LogAsync(string action, string entityName, string? recordId = null,
        string? oldValue = null, string? newValue = null)
    {
        var context = _http.HttpContext;
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = context?.User.FindFirstValue(ClaimTypes.NameIdentifier),
            Action = action,
            EntityName = entityName,
            RecordId = recordId,
            OldValue = oldValue,
            NewValue = newValue,
            IpAddress = context?.Connection.RemoteIpAddress?.ToString(),
            CreatedDate = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }
}
