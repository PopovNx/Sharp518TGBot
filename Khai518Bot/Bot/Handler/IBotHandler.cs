namespace Khai518Bot.Bot.Handler;

public interface IBotHandler
{
    public Task HandleUpdate(Update update);
}