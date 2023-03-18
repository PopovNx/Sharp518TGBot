using Khai518Bot.Bot.Commands.Attributes;

namespace Khai518Bot.Bot.Commands.Entity.Query;

[UsedImplicitly]
[QueryCommand(@$"{QueryName}:(\d+)")]
public class OpenLessonQuery : Command
{
    private const string QueryName = "openlesson";
    public static string Generate(int lessonId) => $"{QueryName}:{lessonId}";

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