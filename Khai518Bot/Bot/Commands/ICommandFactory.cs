namespace Khai518Bot.Bot.Commands;

public interface ICommandFactory
{
    IEnumerable<Command> CreateCommands(Update update);
}