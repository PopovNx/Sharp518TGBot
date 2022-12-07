namespace Khai518Bot.Time;

public class TimeProvider : ITimeProvider
{
    public DateTime Ukraine => DateTime.UtcNow.AddHours(2);
}