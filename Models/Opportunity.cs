using System.ComponentModel.DataAnnotations;

namespace AcxiomCRM.Models;

public class Opportunity
{
    public int OpportunityId { get; set; }
    [Required, StringLength(140)]
    public string OpportunityName { get; set; } = "";
    public int? CustomerId { get; set; }
    public Customer? Customer { get; set; }
    public int? LeadId { get; set; }
    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Amount { get; set; }
    [Range(0, 100)]
    public int Probability { get; set; } = 50;
    [Required, StringLength(30)]
    public string Stage { get; set; } = "Qualification";
    [DataType(DataType.Date)]
    public DateTime ExpectedCloseDate { get; set; } = DateTime.Today.AddDays(30);
    public string Status { get; set; } = "Open";
    public string? AssignedTo { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public decimal WeightedValue => Amount * Probability / 100m;
}
