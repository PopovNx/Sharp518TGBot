using Khai518Bot;
using Khai518Bot.Bot;
using Khai518Bot.Time;
using Microsoft.EntityFrameworkCore;

namespace ServiceTest;

public class NominatorDenominatorTest
{
    private MockTimeProvider _timeProvider = null!;
    private Service _service = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<BotDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        var context = new BotDbContext(options);
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        _timeProvider = new MockTimeProvider();
        _service = new Service(context, _timeProvider);
    }

    [Test]
    public void StartDate()
    {
        var startDate = Service.StartDate;
        Assert.AreEqual(new DateTime(2022, 9, 12), startDate);
    }
    [Test]
    public void IsDenominatorNowStart()
    {
        _timeProvider.SetTime(new DateTime(2022, 9, 13));
        var startDate = _service.IsDenominator;
        Assert.AreEqual(false, startDate);
    }
    [Test]
    public void IsDenominatorNowDen1()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 5));
        Assert.AreEqual(false, _service.IsDenominator);
    }
    [Test]
    public void IsDenominatorNowDen2()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 4));
        Assert.AreEqual(true, _service.IsDenominator);
    }
    [Test]
    public void IsDenominatorNowDen3()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 16));
        Assert.AreEqual(true, _service.IsDenominator);
    }
    [Test]
    public void IsDenominatorShow1()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 4));
        Assert.AreEqual(false, _service.IsDenominatorShow);
    }
    [Test]
    public void IsDenominatorShow2()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 3));
        Assert.AreEqual(false, _service.IsDenominatorShow);
    }
    [Test]
    public void IsDenominatorWeek()
    {
        for (var day = 5; day <= 11; day++)
        {
            _timeProvider.SetTime(new DateTime(2022, 12, day));
            Assert.AreEqual(false, _service.IsDenominator);
        }
        for (var day = 12; day <= 18; day++)
        {
            _timeProvider.SetTime(new DateTime(2022, 12, day));
            Assert.AreEqual(true, _service.IsDenominator);
        }
     
    }
    [Test]
    public void ShowDay1()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 5));
        Assert.AreEqual(1, _service.TodayShowId);
    }
    [Test]
    public void ShowDay2()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 7));
        Assert.AreEqual(3, _service.TodayShowId);
    }
    [Test]
    public void ShowDay3()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 9));
        Assert.AreEqual(5, _service.TodayShowId);
    }
    
    [Test]
    public void ShowDay4()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 10));
        Assert.AreEqual(1, _service.TodayShowId);
    }
    [Test]
    public void ShowDay5()
    {
        _timeProvider.SetTime(new DateTime(2022, 12, 11));
        Assert.AreEqual(1, _service.TodayShowId);
    }
}