using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers;

public class UserController(IUserService userService) : CustomControllerBase
{
    [HttpGet("csv-sync")]
    public async Task<IActionResult> GetCSVUsers()
    {
        var csvUsers = await userService.GetInternalCSVUsers();
        return SuccessResponseWithData(csvUsers);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("internal")]
    public async Task<IActionResult> GetInternalUsers(
        [FromQuery] InternalUserPaginationRequest request
    )
    {
        var users = await userService.GetInternalUsers(request);
        return SuccessResponseWithData(users);
    }

    [Authorize(Policy = "Admin")]
    [HttpGet("external")]
    public async Task<IActionResult> GetExternalUsers(
        [FromQuery] ExternalUserPaginationRequest request
    )
    {
        var users = await userService.GetExternalUsers(request);
        return SuccessResponseWithData(users);
    }

    [Authorize(Policy = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await userService.DeleteUser(id);
        return SuccessResponse();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("internal")]
    public async Task<IActionResult> CreateInternalUser(CreateInternalUserRequest request)
    {
        await userService.CreateInternalUser(request);
        return SuccessResponse();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("external")]
    public async Task<IActionResult> CreateExternalUser(CreateExternalUserRequest request)
    {
        await userService.CreateExternalUser(request);
        return SuccessResponse();
    }

    [Authorize(Policy = "Admin")]
    [HttpPost("internal/batch")]
    public async Task<IActionResult> CreateInternalUsers(CreateInternalUsersRequest request)
    {
        await userService.CreateInternalUsers(request.UsersCsv);
        return SuccessResponse();
    }

    [Authorize(Policy = "Admin")]
    [HttpPatch("internal/{id}")]
    public async Task<IActionResult> UpdateInternalUser(Guid id, UpdateInternalUserRequest request)
    {
        await userService.UpdateInternalUser(id, request);
        return SuccessResponse();
    }

    [Authorize(Policy = "Admin")]
    [HttpPatch("external/{id}")]
    public async Task<IActionResult> UpdateExternalUser(Guid id, UpdateExternalUserRequest request)
    {
        await userService.UpdateExternalUser(id, request);
        return SuccessResponse();
    }
}
