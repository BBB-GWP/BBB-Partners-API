using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class UpdateExternalUserRequest
{
    public string? Email { get; set; }
    public Source? UserSource { get; set; }
}
