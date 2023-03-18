using Khai518Bot.Bot.Commands.Attributes;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[TextCommand(@"editday", @"Редактировать день")]
public class EditDay : Command
{
    public override async Task Execute(Service service)
    {
        var cmd = Message.Text!.Split(' ');
        if (cmd.Length != 2 || !int.TryParse(cmd[1], out var day))
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, "Usage: /editday <day:int(1-5)>");
            return;
        }

        var text = await service.GetOneDayText(day);
        var keyboard = await service.GetEditKeyboard(day, null);
        await BotClient.SendTextMessageAsync(Update.Message!.Chat.Id,
            text, ParseMode.Html, replyMarkup: keyboard);
    }
}