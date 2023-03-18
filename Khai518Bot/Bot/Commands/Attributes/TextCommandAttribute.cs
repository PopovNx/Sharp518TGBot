namespace Khai518Bot.Bot.Commands.Attributes;

public sealed class TextCommandAttribute : CommandAttribute
{
    public string Command { get; }
    public string Description { get; }

    public TextCommandAttribute(string command, string description)
    {
        Command = command;
        Description = description;
    }
}