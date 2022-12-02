using Khai518Bot.Bot.Commands;

namespace Khai518Bot.Bot.Handler;

public class BotHandler : IBotHandler
{
    private readonly ICommandFactory _commandFactory;
    private readonly Service _service;

    public BotHandler(ICommandFactory commandFactory, Service service)
    {
        _commandFactory = commandFactory;
        _service = service;
    }

    public async Task HandleUpdate(Update update)
    {
        foreach (var command in _commandFactory.CreateCommands(update))
            await command.Execute(_service);
    }
}