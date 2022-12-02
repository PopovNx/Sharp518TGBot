namespace Khai518Bot.Bot.Settings;

public interface IBotSettings
{
    public string Token { get; }
    public string WebhookUrl { get; }
    public string DbConnection { get; }
}