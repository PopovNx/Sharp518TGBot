using Khai518Bot.Bot.Handler;
using Khai518Bot.Bot.Settings;
using Microsoft.AspNetCore.Mvc;

namespace Khai518Bot.Controllers;

[ApiController]
[Route("[controller]")]
public class BotWebhookController : ControllerBase
{
    private readonly IBotHandler _botHandler;
    private readonly IBotSettings _botSettings;

    public BotWebhookController(IBotHandler botHandler, IBotSettings botSettings)
    {
        _botHandler = botHandler;
        _botSettings = botSettings;
    }

    [HttpPost("{token}")]
    public async Task<IActionResult> PostAsync([FromBody] Update update, string token)
    {
        if (token == _botSettings.Token)
            await _botHandler.HandleUpdate(update);
        else
            return BadRequest();
        return Ok();
    }
}