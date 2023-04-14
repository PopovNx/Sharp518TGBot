using Khai518Bot.Bot.Commands.Attributes;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[TextCommand(@"getrosp", @"Получить расписание на сегодня", -2)]
public class GetRosp : Command
{
    public override async Task Execute(Service service)
    {
        var text = await service.GetOneDayText(service.TodayShowId);
        var keyboard = await service.GetOneDayKeyboard(service.TodayShowId);
        await BotClient.SendTextMessageAsync(Update.Message!.Chat.Id,
            text, ParseMode.Html, replyMarkup: keyboard);
    }
}