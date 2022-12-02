using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(UpdateType.CallbackQuery, QueryName)]
public class EditDayQuery : Command
{
    public const string QueryName = "editlessonday";

    public enum EditQueryType
    {
        LessonPairMode,
        OpenLessonEditor
    }

    public override async Task Execute(Service service)
    {
        var data = CallbackData.Split(':');
        if (!int.TryParse(data[1], out var mode) || !int.TryParse(data[2], out var dayId) ||
            !int.TryParse(data[3], out var i))
            return;
        switch ((EditQueryType)mode)
        {
            case EditQueryType.LessonPairMode:
            {
                await service.ChangeLessonPairMode(dayId, i);
                var text = await service.GetOneDayText(dayId);
                var keyboard = await service.GetEditKeyboard(dayId, null);
                await TryEditMessage(text, keyboard);
            }
                break;
            case EditQueryType.OpenLessonEditor:
            {
                var pairId = 0;
                var (main, second) = await service.GetLecturesFromLessonPair(dayId, i);

                if (data.Length == 5 && int.TryParse(data[4], out var id))
                    pairId = id;

                var text = await service.GetEditLessonText((pairId == 0 ? main : second).Id);
                var keyboard = await service.GetEditKeyboard(dayId, CallbackData);
                await TryEditMessage(text, keyboard);
                break;
            }
            default:
                return;
        }
    }
}