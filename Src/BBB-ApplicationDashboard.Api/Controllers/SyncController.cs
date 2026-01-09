using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BBB_ApplicationDashboard.Api.Controllers
{
    public class SyncController(IMainServerClient mainServerClient) : CustomControllerBase
    {
        [HttpGet("business/{bid}")]
        public async Task<IActionResult> SyncBusiness(int bid)
        {
            await mainServerClient.SendSyncBid(bid);
            return SuccessResponse();
        }
    }
}
