using System;

namespace BBB_ApplicationDashboard.Application.DTOs.Sync;

public class FailureUpdateRequest
{
    public string? Description { get; set; }
    public bool? IsResolved { get; set; }
}
