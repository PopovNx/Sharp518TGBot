using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command("get_repos_links")]

public class GetRepos : Command
{
    public override async Task Execute(Service service)
    {
        var text = service.GetReposLinks();
        await BotClient.SendTextMessageAsync(Message.Chat, text, parseMode: ParseMode.Html);
    }
}