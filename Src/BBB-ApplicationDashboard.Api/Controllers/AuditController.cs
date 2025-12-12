using BBB_ApplicationDashboard.Application.DTOs.Audit;
using BBB_ApplicationDashboard.Application.DTOs.PaginatedDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    public class AuditController(IAuditService auditService, IN8NAuditService n8n)
        : CustomControllerBase
    {
        [HttpPost("log")]
        public async Task<IActionResult> LogAudit(ActivityEvent activityEvent)
        {
            await auditService.LogActivityEvent(activityEvent);
            return SuccessResponse("Audit logged successfully");
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetAudit(Guid id)
        {
            var audit = await auditService.GetActivityEventById(id);
            return SuccessResponseWithData(audit);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet]
        public async Task<IActionResult> GetAllFilteredAudits(
            [FromQuery] AuditPaginationRequest request
        )
        {
            var audits = await auditService.GetAllFilteredActivityEvents(request);
            return SuccessResponseWithData(audits);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await auditService.GetUsers();
            return SuccessResponseWithData(users);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("statuses")]
        public async Task<IActionResult> GetStatuses()
        {
            var statuses = await auditService.GetStatuses();
            return SuccessResponseWithData(statuses);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("user-versions")]
        public async Task<IActionResult> GetUserVersions()
        {
            var versions = await auditService.GetUserVersions();
            return SuccessResponseWithData(versions);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("entities")]
        public async Task<IActionResult> GetEntities()
        {
            var entities = await auditService.GetEntities();
            return SuccessResponseWithData(entities);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("actions")]
        public async Task<IActionResult> GetActions()
        {
            var actions = await auditService.GetActions();
            return SuccessResponseWithData(actions);
        }

        [Authorize(Policy = "Internal")]
        [HttpPost("status-line-chart-data")]
        public async Task<IActionResult> GetChart([FromBody] AuditLineChartRequest request)
        {
            var data = await auditService.GetActivityLineChartDataGrouped(request);
            return SuccessResponseWithData(data);
        }

        [Authorize(Policy = "Internal")]
        [HttpGet("top-three-users")]
        public async Task<IActionResult> GetTopThreeUsers()
        {
            var actions = await auditService.GetTopUsersPerEntityWithStatus();
            return SuccessResponseWithData(actions);
        }

        [HttpGet("n8n/log")]
        public async Task<IActionResult> LogN8NEvent([FromBody] Dictionary<string, object> payload)
        {
            await n8n.Add(payload);
            return SuccessResponse("N8N Audit logged successfully");
        }
    }
}
