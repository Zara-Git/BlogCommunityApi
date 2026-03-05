using Microsoft.AspNetCore.Mvc;              // ASP.NET Core MVC – Controller, IActionResult, Ok() m.m.

namespace BlogCommunityApi.Controllers;

[ApiController]                              // API-konventioner: bättre binding/validering och standardbeteenden
[Route("api/[controller]")]                  // Bas-route: /api/Test (controller-namnet styr av klassnamnet)
public class TestController : ControllerBase
{
    // GET /api/Test
    [HttpGet]
    public IActionResult Ping() => Ok("API is working ");
    // Ping-endpoint för snabb kontroll att API:t kör:
    // - Om detta svarar 200 OK så fungerar servern och routing
    // - Bra för felsökning när Swagger/Postman inte fungerar
}
