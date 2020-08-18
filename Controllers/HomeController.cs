using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RedisLeaderboardSample 
{
    [Route("api/[controller]")]
    public class TopicsController : Controller 
    {
        private readonly LeaderboardService _service;

        public TopicsController(LeaderboardService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var topics = await _service.GetTopics(0, 0);

            return Ok(topics);
        }

        [HttpPost("{id:int}/replies")]
        public async Task<IActionResult> PostReply(int id)
        {
            await _service.CreateReply(id);

            return NoContent();
        }
    }
}