using System;
using System.Linq.Expressions;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.Sync;
using BBB_ApplicationDashboard.Application.Interfaces;
using BBB_ApplicationDashboard.Domain.Entities;
using BBB_ApplicationDashboard.Domain.ValueObjects;
using BBB_ApplicationDashboard.Infrastructure.Data.Context;
using BBB_ApplicationDashboard.Infrastructure.Exceptions.Common;
using Microsoft.EntityFrameworkCore;

namespace BBB_ApplicationDashboard.Infrastructure.Services.Sync;

public class SyncService(ApplicationDbContext context) : ISyncService
{
    public async Task<PaginatedResponse<InternalFailureResponse>> GetInternalFailures(
        InternalFailurePaginationRequest request
    )
    {
        //! 1) Get all failures
        var query = context.WorkflowSetupFailures.AsNoTracking();

        //! 2) filter by isResolved
        if (request.IsResolved.HasValue)
            query = query.Where(u => u.IsResolved == request.IsResolved.Value);

        //! 3) get total count
        int total = await query.CountAsync();

        //! 4) Apply pagination
        int pageIndex = request.PageNumber - 1;
        int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

        //! 5) Sorting (safe + stable)
        if (
            !string.IsNullOrWhiteSpace(request.SortBy)
            && SortMap.TryGetValue(request.SortBy.Trim(), out var sortExpr)
        )
            query =
                request.SortDirection == SortDirection.Asc
                    ? query.OrderBy(sortExpr).ThenByDescending(f => f.CreatedAt)
                    : query.OrderByDescending(sortExpr).ThenByDescending(f => f.CreatedAt);
        else
            query = query.OrderByDescending(f => f.CreatedAt);

        //! 6) Execute + map
        var failures = await query
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .Select(f => new InternalFailureResponse
            {
                Id = f.WorkflowSetupFailureID,
                IsResolved = f.IsResolved,
                CreatedAt = f.CreatedAt,
                HubSpotID = f.HubSpotID,
                Description = f.Description,
                Type = f.Type,
            })
            .ToListAsync();

        //! 7) Return
        return new PaginatedResponse<InternalFailureResponse>(pageIndex, pageSize, total, failures);
    }

    public async Task<PaginatedResponse<AdminFailureResponse>> GetAdminFailures(
        AdminFailurePaginationRequest request
    )
    {
        //! 1) Get all failures
        var query = context.WorkflowSetupFailures.AsNoTracking();

        //! 2) filter by isResolved
        if (request.IsResolved.HasValue)
            query = query.Where(u => u.IsResolved == request.IsResolved.Value);

        //! 3) get total count
        int total = await query.CountAsync();

        //! 4) Apply pagination
        int pageIndex = request.PageNumber - 1;
        int pageSize = Math.Max(1, Math.Min(100, request.PageSize));

        //! 5) Sorting (safe + stable)
        if (
            !string.IsNullOrWhiteSpace(request.SortBy)
            && SortMap.TryGetValue(request.SortBy.Trim(), out var sortExpr)
        )
            query =
                request.SortDirection == SortDirection.Asc
                    ? query.OrderBy(sortExpr).ThenByDescending(f => f.CreatedAt)
                    : query.OrderByDescending(sortExpr).ThenByDescending(f => f.CreatedAt);
        else
            query = query.OrderByDescending(f => f.CreatedAt);

        //! 6) Execute + map
        var failuresEntities = await query.Skip(pageIndex * pageSize).Take(pageSize).ToListAsync();

        var failures = failuresEntities
            .Select(f => new AdminFailureResponse
            {
                Id = f.WorkflowSetupFailureID,
                IsResolved = f.IsResolved,
                CreatedAt = f.CreatedAt,
                HubSpotID = f.HubSpotID,
                Description = f.Description,
                Type = f.Type,
                ExecutionUrl = f.ExecutionUrl,
                FailureReason = f.FailureReason,
                ResolvedAt = f.ResolvedAt,
                ResolvedBy = f.ResolvedBy,
                UpdatedAt = f.UpdatedAt,
            })
            .ToList();

        //! 7) Return
        return new PaginatedResponse<AdminFailureResponse>(pageIndex, pageSize, total, failures);
    }

    public async Task UpdateFailure(Guid id, FailureUpdateRequest request, string email)
    {
        //! 1) find failure by id
        var failure =
            await context.WorkflowSetupFailures.FindAsync(id)
            ?? throw new NotFoundException($"Failure not found.");

        //! 2) update description if provided
        if (request.Description != null)
            failure.Description = request.Description;

        //! 3) update isResolved if provided
        if (request.IsResolved.HasValue)
        {
            failure.IsResolved = request.IsResolved.Value;
            if (failure.IsResolved)
            {
                failure.ResolvedAt = DateTime.UtcNow;
                failure.ResolvedBy = email;
            }
            else
            {
                failure.ResolvedAt = null;
                failure.ResolvedBy = null;
            }
        }
        failure.UpdatedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
    }

    private static readonly Dictionary<
        string,
        Expression<Func<WorkflowSetupFailure, object>>
    > SortMap = new(StringComparer.OrdinalIgnoreCase)
    {
        ["CreatedAt"] = f => f.CreatedAt,
        ["UpdatedAt"] = f => f.UpdatedAt,
        ["ResolvedAt"] = f => f.ResolvedAt,
        ["IsResolved"] = f => f.IsResolved,
        ["Description"] = f => f.Description ?? "",
        ["FailureReason"] = f => f.FailureReason ?? "",
        ["HubSpotID"] = f => f.HubSpotID,
        ["Type"] = f => f.Type,
        ["ExecutionUrl"] = f => f.ExecutionUrl ?? "",
    };
}
