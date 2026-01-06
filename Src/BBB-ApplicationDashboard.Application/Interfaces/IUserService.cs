using System;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.User;
using BBB_ApplicationDashboard.Domain.Entities;

namespace BBB_ApplicationDashboard.Application.Interfaces;

public interface IUserService
{
    Task<User?> FindUser(string email);
    Task CreateUser(User user);
    Task<PaginatedResponse<InternalUserResponse>> GetInternalUsers(
        InternalUserPaginationRequest request
    );
    Task<PaginatedResponse<ExternalUserResponse>> GetExternalUsers(
        ExternalUserPaginationRequest request
    );
    Task DeleteUser(Guid id);
    Task CreateInternalUser(CreateInternalUserRequest request);
    Task CreateExternalUser(CreateExternalUserRequest request);
    Task CreateInternalUsers(string usersCsv);
    Task UpdateInternalUser(Guid id, UpdateInternalUserRequest request);
    Task UpdateExternalUser(Guid id, UpdateExternalUserRequest request);
    Task<List<string>> GetInternalCSVUsers();
}
