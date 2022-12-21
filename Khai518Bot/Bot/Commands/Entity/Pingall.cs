using Microsoft.EntityFrameworkCore;
using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(@"ping_all")]

public class PingAll : Command
{
    private async Task<string> ToPing(long chatId)
    {
        try
        {
            var users = await _client.GetChatAdministratorsAsync(chatId);
            var ans = users.Where(x => !x.User.IsBot).Select(x => $"[.](tg://user?id={x.User.Id})")
                .Aggregate((x, y) => x + y);
            return ans;
        }
        catch (Exception)
        {
            return "";
        }
    }
    public override async Task Execute(Service service)
    {
        var chat = Message.Chat.Id;
        var toPing = await ToPing(chat);
        await BotClient.SendTextMessageAsync(chat, $"Рубель зовёт Вас!{toPing}");
    }
}
