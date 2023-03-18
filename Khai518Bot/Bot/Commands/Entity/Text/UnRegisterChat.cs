using Khai518Bot.Bot.Commands.Attributes;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[UsedImplicitly]
[TextCommand(@"unregister", @"Отписаться от рассылки")]
public class UnRegisterChat : Command
{
    public override async Task Execute(Service service)
    {
        var chatId = Message.Chat.Id;
        var result = await service.UnregisterChat(chatId);
        var message = result ? "Chat unregistered" : "Chat already unregistered";
        await BotClient.SendTextMessageAsync(chatId, message);
    }
}