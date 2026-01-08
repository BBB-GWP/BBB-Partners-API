using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class InternalUserPaginationRequest : BasePaginationRequest
{
    public bool? IsAdmin { get; set; }
    public bool? IsCSVSync { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; }
    public SortDirection SortDirection { get; set; } = SortDirection.Desc;
}
