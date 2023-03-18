using Khai518Bot.Bot.Commands.Attributes;

namespace Khai518Bot.Bot.Commands.Entity.Text;

[TextCommand(@"setvalue", @"Установить значение")]
public class SetValue : Command
{
    public override async Task Execute(Service service)
    {
        //Edit: /setvalue 3 (1-4) string
        var cmd = Message.Text!.Split(' ');
        if (cmd.Length < 4)
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, @"Недостаточно аргументов");
            return;
        }
        
        if (!int.TryParse(cmd[1], out var id))
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, @"Неверный формат id");
            return;
        }
        
        if (!int.TryParse(cmd[2], out var value) || value is < 1 or > 4)
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, @"Неверный формат значения");
            return;
        }
        
        var val = string.Join(' ', cmd[3..]);
        try
        {
            await service.SetLessonValue(id, value, val);
            await BotClient.DeleteMessageAsync(Message.Chat.Id, Message.MessageId);
        }
        catch (Exception e)
        {
            await BotClient.SendTextMessageAsync(Message.Chat.Id, e.Message);
        }
    }
}