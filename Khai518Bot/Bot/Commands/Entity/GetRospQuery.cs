using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(UpdateType.CallbackQuery, QueryName)]
public class GetRospQuery : Command
{
    public const string QueryName = "aboutday";

    public override async Task Execute(Service service)
    {
        var num = NumberFromQuery();
        if (!num.HasValue) return;

        var text = await service.GetOneDayText(num.Value);
        var keyboard = await service.GetOneDayKeyboard(num.Value);
        await TryEditMessage(text, keyboard);
    }
}