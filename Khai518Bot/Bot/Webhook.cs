using Khai518Bot.Bot.Commands;
using Khai518Bot.Bot.Settings;

namespace Khai518Bot.Bot;

public class Webhook : IHostedService
{
    private readonly ILogger<Webhook> _logger;
    private readonly IBotSettings _settings;
    private readonly ITelegramBotClient _client;

    public Webhook(ILogger<Webhook> logger, IBotSettings settings, ITelegramBotClient client,ICommandFactory commandFactory)
    {
        _logger = logger;
        _settings = settings;
        _client = client;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting webhook");
        await _client.SetWebhookAsync($"{_settings.WebhookUrl}/{_settings.Token}", dropPendingUpdates: true,
            cancellationToken: cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping webhook");
        await _client.DeleteWebhookAsync(cancellationToken: cancellationToken);
    }
}