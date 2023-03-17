using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command("remove_repos_link")]
public class RemoveLink : Command
{
    public override async Task Execute(Service service)
    {
        string text = service.RemoveLink(Message.Text);
        await BotClient.SendTextMessageAsync(Message.Chat, text, parseMode: ParseMode.Html);
    }
}