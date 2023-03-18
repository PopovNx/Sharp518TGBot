using System.Reflection;
using System.Text.RegularExpressions;
using Khai518Bot.Bot.Commands.Attributes;
using Khai518Bot.Bot.Commands.Entity;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands;

public class CommandFactory : ICommandFactory
{
    private readonly ILogger<CommandFactory> _logger;
    private readonly ITelegramBotClient _botClient;

    private readonly Dictionary<CommandAttribute, Type> _commands = new();

    public CommandFactory(ILogger<CommandFactory> logger, ITelegramBotClient botClient)
    {
        _logger = logger;
        _botClient = botClient;
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        var commandTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsSubclassOf(typeof(Command)) && !t.IsAbstract);

        foreach (var type in commandTypes)
        {
            var attribute = type.GetCustomAttribute<CommandAttribute>();
            if (attribute == null)
            {
                _logger.LogWarning("Command {Command} has no attribute", type.Name);
                continue;
            }
            
            _commands.Add(attribute, type);
        }

        var textCommands = _commands.Select(x => x.Key).Where(x => x is TextCommandAttribute)
            .Cast<TextCommandAttribute>();
        var botCommands = textCommands.Select(attribute => new BotCommand
            { Command = attribute.Command, Description = attribute.Description }).ToList();
        foreach (var botCommand in botCommands)
        {
            _logger.LogInformation("Command {Command} added", botCommand.Command);
        }

        _botClient.SetMyCommandsAsync(botCommands);
    }

    private static bool ShouldBeInvoked(CommandAttribute attribute, Update update)
    {
        switch (attribute)
        {
            case TextCommandAttribute textCommand when update.Type == UpdateType.Message:
                return update.Message?.Text?.StartsWith($"/{textCommand.Command}") ?? false;
            
            case QueryCommandAttribute queryCommand when update.Type == UpdateType.CallbackQuery:
                var patten = new Regex(queryCommand.Pattern);
                return patten.IsMatch(update.CallbackQuery?.Data ?? string.Empty);
        }
        return false;
    }

    public IEnumerable<Command> CreateCommands(Update update)
    {
        foreach (var (_, type) in _commands.Where(x => ShouldBeInvoked(x.Key, update)))
        {
            var instance = Activator.CreateInstance(type);
            if (instance is not Command command) continue;
            command.Init(_botClient, update);
            _logger.LogInformation("Command {TypeName} invoked with {UpdateType} by {User}",
                type.Name, update.Type, update.Message?.From?.Username ?? update.CallbackQuery?.From?.Username);
            yield return command;
        }
    }
}