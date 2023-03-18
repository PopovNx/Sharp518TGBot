
namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command("add_repos_link")]
public class AddLink : Command
{
    public override async Task Execute(Service service)
    {
        var text = service.AddLink(Message.Text);
        await BotClient.SendTextMessageAsync(Message.Chat, text);
    }
}