namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(@"gettime")]
public class GetTime : Command
{
    public override async Task Execute(Service service)
    {
        await BotClient.SendTextMessageAsync(Message.Chat, service.TimeStr);
    }
}