using System.ComponentModel.DataAnnotations;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.User;

public class CreateExternalUserRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required Source UserSource { get; set; }
}
