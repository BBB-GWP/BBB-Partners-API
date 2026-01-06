using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class ExternalUserResponse
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public Source UserSource { get; set; }
}
