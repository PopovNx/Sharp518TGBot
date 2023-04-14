using Khai518Bot.Bot.Commands.Attributes;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[TextCommand(@"pingall", @"Пингануть всех в чате",-1)]

public class PingAll : Command
{
    public override async Task Execute(Service service)
    {
        var users = await BotClient.GetChatAdministratorsAsync(Message.Chat);
        var toPing = users.Where(x => !x.User.IsBot).Select(x => $"[.](tg://user?id={x.User.Id})")
            .Aggregate((x, y) => x + y);
        await BotClient.SendTextMessageAsync(Message.Chat, $"Рубель зовёт Вас!{toPing} (макс где вебхуки)", ParseMode.Markdown);
    }
}
