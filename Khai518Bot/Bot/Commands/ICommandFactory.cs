using Khai518Bot.Bot.Commands.Entity;

namespace Khai518Bot.Bot.Commands;

public interface ICommandFactory
{
    IEnumerable<Command> CreateCommands(Update update);
}