using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class AuditLog
{
    public int AuditLogId { get; set; }
    public string? UserId { get; set; }
    [Required, StringLength(60)]
    public string Action { get; set; } = "";
    [Required, StringLength(80)]
    public string EntityName { get; set; } = "";
    public string? RecordId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    [StringLength(64)]
    public string? IpAddress { get; set; }
}
