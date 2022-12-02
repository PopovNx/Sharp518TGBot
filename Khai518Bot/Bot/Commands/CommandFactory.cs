using System.Reflection;
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
    }

    private static bool ShouldBeInvoked(CommandAttribute attribute, Update update)
    {
        if (attribute.UpdateType != update.Type)
            return false;
        if (attribute.UpdateType != UpdateType.Message && attribute.UpdateType != UpdateType.CallbackQuery)
            return true;
        if (!attribute.HasCommand)
            return true;
        if (!string.IsNullOrEmpty(update.Message?.Text) && update.Message.Text.StartsWith($"/{attribute.Command}"))
            return true;
        return !string.IsNullOrEmpty(update.CallbackQuery?.Data) &&
               update.CallbackQuery.Data.StartsWith($"{attribute.Command}");
    }

    public IEnumerable<Command> CreateCommands(Update update)
    {
        foreach (var (_, type) in _commands.Where(x => ShouldBeInvoked(x.Key, update)))
        {
            var instance = Activator.CreateInstance(type);
            if (instance is not Command command) continue;
            command.Update = update;
            command.BotClient = _botClient;
            _logger.LogInformation("Command {TypeName} invoked", type.Name);
            yield return command;
        }
    }
}