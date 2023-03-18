namespace Khai518Bot.Bot.Commands.Attributes;

public sealed class QueryCommandAttribute : CommandAttribute
{
    public string Pattern { get; }

    public QueryCommandAttribute(string pattern) => Pattern = pattern;
}