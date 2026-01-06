using System;
using BBB_ApplicationDashboard.Domain.ValueObjects;

namespace BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;

public class ExternalUserPaginationRequest : BasePaginationRequest
{
    public Source? UserSource { get; set; }
}
