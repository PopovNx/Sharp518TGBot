using Khai518Bot.Bot.Commands.Attributes;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[TextCommand(@"getweek", @"Получить расписание на неделю", -2)]
public class GetWeek : Command
{
    public override async Task Execute(Service service)
    {
        var text = await service.GetOneDayText(service.TodayShowId);
        var keyboard = await service.GetDaysKeyboard(service.TodayShowId);
        await BotClient.SendTextMessageAsync(Update.Message!.Chat.Id,
            text, ParseMode.Html, replyMarkup: keyboard);
    }
}