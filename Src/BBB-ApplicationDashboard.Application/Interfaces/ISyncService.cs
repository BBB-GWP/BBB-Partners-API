using System;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.Sync;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface ISyncService
{
    Task<PaginatedResponse<InternalFailureResponse>> GetInternalFailures(
        InternalFailurePaginationRequest request
    );

    Task UpdateFailure(Guid id, FailureUpdateRequest request, string email);

    Task<PaginatedResponse<AdminFailureResponse>> GetAdminFailures(
        AdminFailurePaginationRequest request
    );
}
