using Khai518Bot.Bot.Commands.Attributes;

namespace Khai518Bot.Bot.Commands.Entity.Query;

[QueryCommand(@$"{QueryName}:(\d+)")]
public class GetWeekQuery : Command
{
    private const string QueryName = "select_day";
    public static string Generate(int dayId) => $"{QueryName}:{dayId}";


    public override async Task Execute(Service service)
    {
        var num = NumberFromQuery();
        if (!num.HasValue) return;

        var text = await service.GetOneDayText(num.Value);
        var keyboard = await service.GetDaysKeyboard(num.Value);
        await TryEditMessage(text, keyboard);
    }
}