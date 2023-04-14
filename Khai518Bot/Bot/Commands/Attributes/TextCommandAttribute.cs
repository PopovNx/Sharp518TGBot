namespace Khai518Bot.Bot.Commands.Attributes;

public sealed class TextCommandAttribute : CommandAttribute
{
    public string Command { get; }
    public string Description { get; }
    public int Priority { get; set; }

    public TextCommandAttribute(string command, string description, int priority = 0)
    {
        Command = command;
        Description = description;
        Priority = priority;
    }
}