using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot;

public class LessonsListen : IHostedService
{
    private readonly ILogger<LessonsListen> _logger;
    private readonly Service _service;
    private readonly ITelegramBotClient _client;
    private readonly BotDbContext _context;

    public LessonsListen(ILogger<LessonsListen> logger, Service service, ITelegramBotClient client,
        BotDbContext context)
    {
        _logger = logger;
        _service = service;
        _client = client;
        _context = context;
    }

    private async Task<string> ToPing(long chatId)
    {
        try
        {
            var users = await _client.GetChatAdministratorsAsync(chatId);
            var ans = users.Where(x => !x.User.IsBot).Select(x => $"[.](tg://user?id={x.User.Id})")
                .Aggregate((x, y) => x + y);
            return ans;
        }
        catch (Exception)
        {
            return "";
        }
    }

    private async Task DoWork(CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(TimeSpan.FromSeconds(10));

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            var lesson = await _service.GetLessonIn5Minutes();
            if (lesson == null) continue;
            _logger.LogInformation("Lesson in 5 minutes");
            var chats = await _context.ListenChats.ToListAsync(cancellationToken: cancellationToken);
            foreach (var chat in chats)
            {
                var toPing = await ToPing(chat.ChatId);
                await _client.SendTextMessageAsync(chat.ChatId, $"Через 5 минут урок: {lesson.Title}!{toPing}",
                    cancellationToken: cancellationToken, parseMode: ParseMode.Markdown);
            }

            await Task.Delay(1000 * 60 * 6, cancellationToken);
        }
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LessonsListen started");
        Task.Run(() => DoWork(cancellationToken), cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("LessonsListen is stopping");
        return Task.CompletedTask;
    }
}