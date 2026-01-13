using System.Security.Claims;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using BBB_ApplicationDashboard.Application.DTOs.Sync;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    public class SyncController(ISyncService syncService, IUserService userService)
        : CustomControllerBase
    {
        [Authorize(Policy = "Internal")]
        [HttpGet("failures/internal")]
        public async Task<IActionResult> GetInternalFailures(
            [FromQuery] InternalFailurePaginationRequest request
        )
        {
            var failures = await syncService.GetInternalFailures(request);
            return SuccessResponseWithData(failures);
        }

        [Authorize(Policy = "Internal")]
        [HttpPatch("failures/{id}")]
        public async Task<IActionResult> UpdateFailure(Guid id, FailureUpdateRequest request)
        {
            var email = User.FindFirstValue(ClaimTypes.Email)!;
            await syncService.UpdateFailure(id, request, email);
            return SuccessResponse();
        }

        [Authorize(Policy = "Admin")]
        [HttpGet("failures/admin")]
        public async Task<IActionResult> GetAdminFailures(
            [FromQuery] AdminFailurePaginationRequest request
        )
        {
            var failures = await syncService.GetAdminFailures(request);
            return SuccessResponseWithData(failures);
        }
    }
}
