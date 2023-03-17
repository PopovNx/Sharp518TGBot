using Microsoft.EntityFrameworkCore;
using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(@"anton")]

public class Anton : Command
{
    public override async Task Execute(Service service)
    {
        await BotClient.SendTextMessageAsync(Message.Chat, $"танцуем", ParseMode.Markdown);
    }
}
