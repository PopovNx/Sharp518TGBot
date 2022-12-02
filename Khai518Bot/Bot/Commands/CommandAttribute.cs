using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands;

public class CommandAttribute : Attribute
{
    public CommandAttribute(UpdateType updateType, string? command = null)
    {
        UpdateType = updateType;
        Command = command;
    }

    public CommandAttribute(string command) : this(UpdateType.Message, command)
    {
    }

    public UpdateType UpdateType { get; }
    public string? Command { get; }
    public bool HasCommand => !string.IsNullOrEmpty(Command);
}