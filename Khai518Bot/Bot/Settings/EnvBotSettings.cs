namespace Khai518Bot.Bot.Settings;

public class EnvBotSettings :IBotSettings
{
    private ILogger<EnvBotSettings> Logger { get; }
    public string Token { get; }
    public string WebhookUrl { get; }
    public string DbConnection { get; }

    public EnvBotSettings(ILogger<EnvBotSettings> logger)
    {
        Logger = logger;
        var token = Environment.GetEnvironmentVariable("BOT_TOKEN");
        var webhookUrl = Environment.GetEnvironmentVariable("WEBHOOK_URL");
        var dbLocation = Environment.GetEnvironmentVariable("DB_LOCATION");
        if (token is null || webhookUrl is null || dbLocation is null)
            throw new Exception("Environment variables not set");
        Token = token;
        WebhookUrl = webhookUrl;
        DbConnection = dbLocation;
        Logger.LogInformation("BotSettings loaded");
    }

}