namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(@"gettime")]
public class GetTime : Command
{
    public override async Task Execute(Service service)
    {
        var time = Service.TimeInUkraine.ToString("HH:mm:ss");
        await BotClient.SendTextMessageAsync(Message.Chat, time);
    }
}