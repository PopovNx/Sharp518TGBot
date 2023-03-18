using Khai518Bot.Bot.Commands.Attributes;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[TextCommand(@"time", @"Показать серверное время")]
public class GetTime : Command
{
    public override async Task Execute(Service service)
    {
        await BotClient.SendTextMessageAsync(Message.Chat, service.TimeStr);
    }
}