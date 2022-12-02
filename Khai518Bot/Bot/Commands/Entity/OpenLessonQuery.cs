using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity;

[UsedImplicitly]
[Command(UpdateType.CallbackQuery, QueryName)]
public class OpenLessonQuery : Command
{
    public const string QueryName = "openlesson";

    public override async Task Execute(Service service)
    {
        var lesson = CallbackData.Split(":").ElementAtOrDefault(1);
        if (string.IsNullOrEmpty(lesson)) return;
        if (!int.TryParse(lesson, out var lessonId)) return;

        var text = await service.GetLessonText(lessonId);
        var keyboard = await service.GetLessonKeyboard(lessonId);
        await TryEditMessage(text, keyboard);
    }
}