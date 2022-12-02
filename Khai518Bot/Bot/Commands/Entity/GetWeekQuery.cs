using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(UpdateType.CallbackQuery, QueryName)]
public class GetWeekQuery : Command
{
    public const string QueryName = "select_day";

    public override async Task Execute(Service service)
    {
        var num = NumberFromQuery();
        if (!num.HasValue) return;

        var text = await service.GetOneDayText(num.Value);
        var keyboard = await service.GetDaysKeyboard(num.Value);
        await TryEditMessage(text, keyboard);
    }
}