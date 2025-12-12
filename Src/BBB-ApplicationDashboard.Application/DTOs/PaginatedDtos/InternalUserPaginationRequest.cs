using System;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class InternalUserPaginationRequest : BasePaginationRequest
{
    public bool? IsAdmin { get; set; }
    public bool? IsCSVSync { get; set; }
    public bool? IsActive { get; set; }
}
