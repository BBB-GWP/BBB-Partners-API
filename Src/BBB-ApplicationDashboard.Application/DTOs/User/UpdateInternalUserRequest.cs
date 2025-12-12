namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class UpdateInternalUserRequest
{
    public string? Email { get; set; }
    public bool? IsCSVSync { get; set; }
    public bool? IsActive { get; set; }
}
