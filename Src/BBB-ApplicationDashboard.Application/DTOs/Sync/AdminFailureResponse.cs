using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.Sync;

public class AdminFailureResponse
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public required string HubSpotID { get; set; }
    public bool IsResolved { get; set; }
    public string? ResolvedBy { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public EntitySetupFailureType Type { get; set; }
    public string? ExecutionUrl { get; set; }
    public string? FailureReason { get; set; }
}
