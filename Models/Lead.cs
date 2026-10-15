using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Lead
{
    public int LeadId { get; set; }
    [Required, StringLength(120)]
    public string LeadName { get; set; } = "";
    [EmailAddress, StringLength(256)]
    public string? Email { get; set; }
    [Phone, StringLength(20)]
    public string? Phone { get; set; }
    [StringLength(100)]
    public string? CompanyName { get; set; }
    [Required, StringLength(80)]
    public string Source { get; set; } = "Website";
    [Required, StringLength(30)]
    public string Status { get; set; } = "New";
    [Range(0, 1000000000)]
    public decimal ExpectedValue { get; set; }
    public string? AssignedTo { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
