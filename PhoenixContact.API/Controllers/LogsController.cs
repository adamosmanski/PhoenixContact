using Microsoft.AspNetCore.Mvc;

namespace PhoenixContact.API.Controllers
{
    public class LogsController : ControllerBase
    {
        private readonly ILogger<LogsController> _logger;

        public LogsController(ILogger<LogsController> logger)
        {
            _logger = logger;
        }

        [HttpPost("error")]
        public IActionResult ReceiveErrorLog([FromBody] ErrorLogDto log)
        {
            if (log == null)
                return BadRequest();

            _logger.LogError($"Client error: {log.Message} | StackTrace: {log.StackTrace}");

            return Ok();
        }
    }
}
