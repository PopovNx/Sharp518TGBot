namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(@"registerchat")]
public class RegisterChat : Command
{
    public override async Task Execute(Service service)
    {
        var chatId = Message.Chat.Id;
        var result = await service.RegisterChat(chatId);
        var message = result ? "Chat registered" : "Chat already registered";
        await BotClient.SendTextMessageAsync(chatId, message);
    }
}