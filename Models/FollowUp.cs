using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class FollowUp
{
    public int FollowUpId { get; set; }
    public int? CustomerId { get; set; }
    public int? LeadId { get; set; }
    [Required, StringLength(140)]
    public string Subject { get; set; } = "";
    [Required, DataType(DataType.DateTime)]
    public DateTime FollowUpDate { get; set; } = DateTime.Today.AddDays(1);
    [Required, StringLength(30)]
    public string FollowUpType { get; set; } = "Call";
    [Required, StringLength(30)]
    public string Status { get; set; } = "Planned";
    [StringLength(1000)]
    public string? Remarks { get; set; }
    public string? AssignedTo { get; set; }
}
