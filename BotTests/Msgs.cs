using System.Text;
using Khai518Bot;
using Khai518Bot.Bot;
using Khai518Bot.Time;
using Microsoft.EntityFrameworkCore;

namespace ServiceTest;

public class MsgsTest
{
    private MockTimeProvider _timeProvider = null!;
    private Service _service = null!;
    private BotDbContext _dbContext = null!;

    [SetUp]
    public async Task Setup()
    {
        Console.OutputEncoding = Encoding.UTF8;
        var options = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        _dbContext = new BotDbContext(options);
        _timeProvider = new MockTimeProvider();
        _service = new Service(_dbContext, _timeProvider);
        await _service.ChangeLessonPairMode(1, 1);
    }

    [Test]
    public async Task DaysTest()
    {
        var msg = await _dbContext.Days.ToListAsync();
        Assert.AreEqual(5, msg.Count);
    }
    [Test]
    public void GetTimeTest()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 5, 8, 55, 5));
        var str = _service.TimeStr;
        Assert.AreEqual("08:55", str);
    }
    [Test]
    public async Task StartDate()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 5, 8, 55, 5));
        var msg = await _service.GetOneDayText(_service.TodayShowId);
        Console.WriteLine(msg);
    }
}