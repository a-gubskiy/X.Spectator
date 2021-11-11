using Microsoft.AspNetCore.Mvc;

namespace Example.WebApp.Controllers;

[ApiController]
public class DemoController : ControllerBase
{
    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Get()
    {
        return Ok("Hello!");
    }
}