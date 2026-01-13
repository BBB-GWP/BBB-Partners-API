using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.Sync;

public class InternalFailureResponse
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public required string HubSpotID { get; set; }
    public bool IsResolved { get; set; }
    public DateTime CreatedAt { get; set; }
    public EntitySetupFailureType Type { get; set; }
}
