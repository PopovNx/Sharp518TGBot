using Khai518Bot.Models;
using Microsoft.EntityFrameworkCore;

namespace Khai518Bot;

public sealed class BotDbContext : DbContext
{
    public BotDbContext(DbContextOptions<BotDbContext> options) : base(options)
    {
        Database.EnsureCreated();
        if (Days.Any()) return;
        Days.Add(Day.CreateInit(1, @"Понедельник"));
        Days.Add(Day.CreateInit(2, @"Вторник"));
        Days.Add(Day.CreateInit(3, @"Среда"));
        Days.Add(Day.CreateInit(4, @"Четверг"));
        Days.Add(Day.CreateInit(5, @"Пятница"));

        var times = new[]
        {
            new TimeSpan(8, 0, 0), new TimeSpan(9, 35, 0),
            new TimeSpan(9, 50, 0), new TimeSpan(11, 25, 0),
            new TimeSpan(11, 55, 0), new TimeSpan(13, 30, 0),
            new TimeSpan(13, 45, 0), new TimeSpan(15, 20, 0)
        };
        SaveChanges();
        foreach (var day in Days)
        {
            for (var i = 0; i < day.LecturePairs.Count; i++)
            {
                var pair = day.LecturePairs[i];
                pair.TimeStart = times[i * 2];
                pair.TimeEnd = times[i * 2 + 1];
            }
        }

        SaveChanges();
    }

    public DbSet<Day> Days { get; set; } = null!;
    public DbSet<LecturePair> LecturePairs { get; set; } = null!;
    public DbSet<Lecture> Lectures { get; set; } = null!;
    public DbSet<ListenChat> ListenChats { get; set; } = null!;
}