using System;
using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Domain.Entities;

public class WorkflowSetupFailure
{
    [Key]
    public Guid WorkflowSetupFailureID { get; set; }
    public string? Description { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public required string HubSpotID { get; set; }
    public EntitySetupFailureType Type { get; set; }
    public string? ExecutionUrl { get; set; }
    public string? FailureReason { get; set; }
}
