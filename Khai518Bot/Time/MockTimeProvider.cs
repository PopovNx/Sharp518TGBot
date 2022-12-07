namespace Khai518Bot.Time;

public class MockTimeProvider : ITimeProvider
{
    public MockTimeProvider() => Ukraine = DateTime.Now;
    public void SetTime(DateTime now) => Ukraine = now;
    public DateTime Ukraine { get; private set; }
}