using System.Text;
using Khai518Bot.Bot.Commands.Entity;
using Khai518Bot.Models;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types.ReplyMarkups;

namespace Khai518Bot.Bot;

public class Service
{
    private readonly BotDbContext _db;

    public Service(BotDbContext dbContext) => _db = dbContext;

    private async Task<Day> GetDay(int dayId) => await _db.Days
        .Include(x => x.LecturePairs)
        .ThenInclude(x => x.MainLecture)
        .Include(x => x.LecturePairs)
        .ThenInclude(x => x.SubLecture).FirstAsync(x => x.Id == dayId);

    private static bool IsDenominator => TimeInUkraine.DayOfYear / 7 % 2 == 0;
    private static string BoldIf(string text, bool condition) => condition ? $"<b>{text}</b>" : text;
    private static string UnderlineIf(string text, bool condition) => condition ? $"<u>{text}</u>" : text;
    private static string Bold(string text) => BoldIf(text, true);
    private static string BoldUnderlineIf(string t, bool c) => BoldIf(UnderlineIf(t, c), c);

    public static DateTime TimeInUkraine =>
        DateTime.UtcNow.AddHours(2) - TimeSpan.FromHours(8) - TimeSpan.FromMinutes(13);

    public static int TodayShowId
    {
        get
        {
            var dayId = (int)TimeInUkraine.DayOfWeek;
            if (dayId == 0) dayId = 7;
            if (dayId > 5) dayId = 1;
            return dayId;
        }
    }


    public async Task<string> GetOneDayText(int dayId)
    {
        var day = await GetDay(dayId);
        var text = new StringBuilder();
        var lectureNow = await GetLectureOnTime();
        var lectureIn5Min = await GetLessonIn5Minutes();
        var denominatorText = IsDenominator ? @"Знам" : @"Чис";
        text.AppendLine($"Расписание на {Bold(day.Name)} ({denominatorText})\n");

        for (var i = 0; i < day.LecturePairs.Count; i++)
        {
            var lesson = day.LecturePairs[i];
            var (main, sub) = await GetLecturesFromLessonPair(dayId, lesson.Id);

            text.AppendLine($"{Bold($"{i + 1}.")} {lesson.TimeStart:hh\\:mm} - {lesson.TimeEnd:hh\\:mm}:");
            var isMainNow = lectureNow == main ? "🔥" : "";
            var isSubNow = lectureNow == sub ? "🔥" : "";
            isMainNow = lectureIn5Min == main ? "⏰" : isMainNow;
            isSubNow = lectureIn5Min == sub ? "⏰" : isSubNow;
            switch (lesson.TypeState)
            {
                case LecturePair.State.None:
                    text.AppendLine($@">    {Bold(@"Нет Пары")}");
                    break;
                case LecturePair.State.One:
                    text.AppendLine($">  {main.Title ?? @"Нет подписи"} {isMainNow}");
                    break;
                case LecturePair.State.OneNominator:
                    text.AppendLine(
                        $"> Ч:  {BoldUnderlineIf(main.Title ?? @"Нет подписи", !IsDenominator)} {isMainNow}");
                    text.AppendLine($"> З:  {BoldUnderlineIf(@"Нет пары", IsDenominator)} {isMainNow}");
                    break;
                case LecturePair.State.OneDenominator:
                    text.AppendLine($"> Ч:  {BoldUnderlineIf(@"Нет пары", !IsDenominator)} {isMainNow}");
                    text.AppendLine(
                        $"> З:  {BoldUnderlineIf(main.Title ?? @"Нет подписи", IsDenominator)} {isMainNow}");
                    break;
                case LecturePair.State.Two:
                    text.AppendLine(
                        $"> Ч:  {BoldUnderlineIf(main.Title ?? @"Нет подписи", !IsDenominator)} {isMainNow}");
                    text.AppendLine(
                        $"> З:  {BoldUnderlineIf(sub.Title ?? @"Нет подписи", IsDenominator)} {isSubNow}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            text.AppendLine();
        }

        return text.ToString();
    }

    public async Task<string> GetLessonText(int lessonId, bool numerate = false)
    {
        var lesson = await _db.Lectures.FirstOrDefaultAsync(x => x.Id == lessonId);
        if (lesson is null) return @"Нет такой пары";
        var day = await _db.Days.FirstOrDefaultAsync(x =>
            x.LecturePairs.Any(y => y.MainLecture == lesson || y.SubLecture == lesson));
        var lecturePair = _db.LecturePairs.First(x => x.MainLecture == lesson || x.SubLecture == lesson);
        var text = new StringBuilder();
        text.AppendLine($@"День: {Bold(day?.Name ?? @"Нет дня")}");
        text.AppendLine($@"Лекция: {Bold(lecturePair.Number.ToString())}");
        text.AppendLine($@"Начало: {Bold(lecturePair.TimeStart.ToString(@"hh\:mm"))}");
        text.AppendLine($"Конец: {Bold(lecturePair.TimeEnd.ToString(@"hh\:mm"))}\n");
        text.AppendLine($"{(numerate ? "1" : "")}> {Bold(lesson.Title ?? @"Нет подписи")}\n");
        text.AppendLine($"{(numerate ? "2> " : "")}Преподаватель: {Bold(lesson.Teacher ?? @"Нет преподавателя")}\n");
        text.AppendLine($"{(numerate ? "3> " : "")}Платформа: {Bold(lesson.LinkPlatform ?? @"Нет платформы")}\n");
        text.AppendLine($"{(numerate ? "4> " : "")}Ссылка: {Bold(lesson.Link ?? @"Нет ссылки")}");

        return text.ToString();
    }

    private async Task<Lecture?> GetLectureOnTime(DateTime? time = null)
    {
        time ??= TimeInUkraine;
        if (time.Value.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            return null;
        var isDenominator = time.Value.DayOfYear / 7 % 2 == 0;
        var day = await GetDay((int)time.Value.DayOfWeek);
        var onlyTime = time.Value.TimeOfDay;
        var lesson = day.LecturePairs.FirstOrDefault(x => x.TimeStart <= onlyTime && x.TimeEnd >= onlyTime);
        if (lesson is null) return null;
        return lesson.TypeState switch
        {
            LecturePair.State.None => null,
            LecturePair.State.One => lesson.MainLecture,
            LecturePair.State.OneNominator when !isDenominator => lesson.MainLecture,
            LecturePair.State.OneDenominator when isDenominator => lesson.MainLecture,
            LecturePair.State.Two when isDenominator => lesson.SubLecture,
            LecturePair.State.Two when !isDenominator => lesson.MainLecture,
            _ => null
        };
    }

    public async Task<InlineKeyboardMarkup> GetOneDayKeyboard(int dayId)
    {
        var day = await GetDay(dayId);
        var keyboardRaw = new List<List<InlineKeyboardButton>> { new() };
        for (var i = 0; i < day.LecturePairs.Count; i++)
        {
            var lesson = day.LecturePairs[i];
            if (lesson.TypeState == LecturePair.State.None) continue;
            switch (lesson.TypeState)
            {
                case LecturePair.State.One:
                {
                    var lec = lesson.MainLecture ?? lesson.SubLecture!;
                    keyboardRaw[0]
                        .Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}",
                            $"{OpenLessonQuery.QueryName}:{lec.Id}"));
                    break;
                }
                case LecturePair.State.OneNominator:
                {
                    var lec = lesson.MainLecture ?? lesson.SubLecture!;
                    keyboardRaw[0]
                        .Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. Ч",
                            $"{OpenLessonQuery.QueryName}:{lec.Id}"));
                    break;
                }
                case LecturePair.State.OneDenominator:
                {
                    var lec = lesson.MainLecture ?? lesson.SubLecture!;
                    keyboardRaw[0]
                        .Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. З",
                            $"{OpenLessonQuery.QueryName}:{lec.Id}"));
                    break;
                }
                case LecturePair.State.Two:

                    keyboardRaw[0]
                        .Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. Ч",
                            $"{OpenLessonQuery.QueryName}:{lesson.MainLecture?.Id}"));
                    keyboardRaw[0]
                        .Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. З",
                            $"{OpenLessonQuery.QueryName}:{lesson.SubLecture?.Id}"));

                    break;
                case LecturePair.State.None:
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        keyboardRaw.Add(new List<InlineKeyboardButton>
        {
            InlineKeyboardButton.WithCallbackData(@"Назад", $"{GetWeekQuery.QueryName}:{dayId}")
        });
        return new InlineKeyboardMarkup(keyboardRaw);
    }

    public async Task<InlineKeyboardMarkup> GetDaysKeyboard(int dayId)
    {
        var days = await _db.Days.ToListAsync();
        var keyboardRaw = new InlineKeyboardButton[days.Count + 1][];
        keyboardRaw[0] = new[]
            { InlineKeyboardButton.WithCallbackData(@"Подробнее", $"{GetRospQuery.QueryName}:{dayId}") };
        for (var i = 0; i < days.Count; i++)
        {
            var day = days[i];
            var selected = day.Id == dayId ? "✅" : "";
            var today = day.Id == TodayShowId ? "📅" : "";
            keyboardRaw[i + 1] = new[]
            {
                InlineKeyboardButton.WithCallbackData($"{selected} {day.Name} {today}",
                    $"{GetWeekQuery.QueryName}:{day.Id}")
            };
        }

        return new InlineKeyboardMarkup(keyboardRaw);
    }

    public async Task<InlineKeyboardMarkup> GetLessonKeyboard(int lessonId)
    {
        var lesson = await _db.Lectures.FirstOrDefaultAsync(x => x.Id == lessonId);
        var day = await _db.Days.FirstOrDefaultAsync(x =>
            x.LecturePairs.Any(y => y.MainLecture == lesson || y.SubLecture == lesson));
        var keyboardRaw = new List<List<InlineKeyboardButton>>
        {
            new() { InlineKeyboardButton.WithCallbackData(@"Назад", $"{GetRospQuery.QueryName}:{day?.Id}") }
        };
        return new InlineKeyboardMarkup(keyboardRaw);
    }

    public async Task<InlineKeyboardMarkup> GetEditKeyboard(int dayId, string? updateQuery)
    {
        var day = await GetDay(dayId);
        var keyboardRaw = new List<List<InlineKeyboardButton>>();
        if (updateQuery != null)
            keyboardRaw.Add(new List<InlineKeyboardButton>
            {
                InlineKeyboardButton.WithCallbackData(@"Обновить", updateQuery)
            });
        for (var i = 0; i < day.LecturePairs.Count; i++)
        {
            var lesson = day.LecturePairs[i];

            var buttons = new List<InlineKeyboardButton>();
            var modeText = lesson.TypeState switch
            {
                LecturePair.State.None => "Нет",
                LecturePair.State.One => "Одна",
                LecturePair.State.OneNominator => "Одна Ч",
                LecturePair.State.OneDenominator => "Одна З",
                LecturePair.State.Two => "Две",
                _ => throw new ArgumentOutOfRangeException()
            };
            buttons.Add(InlineKeyboardButton.WithCallbackData($@"{modeText}",
                $"{EditDayQuery.QueryName}:{(int)EditDayQuery.EditQueryType.LessonPairMode}:{dayId}:{lesson.Id}"));
            switch (lesson.TypeState)
            {
                case LecturePair.State.One:
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}",
                        $"{EditDayQuery.QueryName}:{(int)EditDayQuery.EditQueryType.OpenLessonEditor}:{dayId}:{lesson.Id}"));
                    break;
                }
                case LecturePair.State.OneNominator:
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. Ч",
                        $"{EditDayQuery.QueryName}:{(int)EditDayQuery.EditQueryType.OpenLessonEditor}:{dayId}:{lesson.Id}"));
                    break;
                }
                case LecturePair.State.OneDenominator:
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. З",
                        $"{EditDayQuery.QueryName}:{(int)EditDayQuery.EditQueryType.OpenLessonEditor}:{dayId}:{lesson.Id}"));
                    break;
                }
                case LecturePair.State.Two:
                {
                    buttons.Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. Ч",
                        $"{EditDayQuery.QueryName}:{(int)EditDayQuery.EditQueryType.OpenLessonEditor}:{dayId}:{lesson.Id}:0"));
                    buttons.Add(InlineKeyboardButton.WithCallbackData($@"{i + 1}. З",
                        $"{EditDayQuery.QueryName}:{(int)EditDayQuery.EditQueryType.OpenLessonEditor}:{dayId}:{lesson.Id}:1"));
                    break;
                }
            }

            keyboardRaw.Add(buttons);
        }

        return new InlineKeyboardMarkup(keyboardRaw);
    }

    public async Task ChangeLessonPairMode(int dayId, int lessonPairId)
    {
        var day = await GetDay(dayId);
        var lessonPair = day.LecturePairs.FirstOrDefault(x => x.Id == lessonPairId);
        if (lessonPair == null) return;
        var newMode = (LecturePair.State)(((int)lessonPair.TypeState + 1) % 5);
        lessonPair.TypeState = newMode;
        await _db.SaveChangesAsync();
    }

    public async Task<(Lecture main, Lecture second)> GetLecturesFromLessonPair(int dayId, int lessonPairId)
    {
        var day = await GetDay(dayId);
        var lessonPair = day.LecturePairs.FirstOrDefault(x => x.Id == lessonPairId);
        if (lessonPair == null) throw new Exception("LessonPair not found");
        if (lessonPair.MainLecture == null)
        {
            lessonPair.MainLecture = new Lecture();
            await _db.SaveChangesAsync();
        }

        if (lessonPair.SubLecture == null)
        {
            lessonPair.SubLecture = new Lecture();
            await _db.SaveChangesAsync();
        }

        return (lessonPair.MainLecture, lessonPair.SubLecture);
    }

    public async Task<string> GetEditLessonText(int lessonId)
    {
        var text = new StringBuilder(await GetLessonText(lessonId, true));
        text.AppendLine();
        text.AppendLine($@"Edit: /setvalue {lessonId} (1-4) string");
        return text.ToString();
    }

    public async Task SetLessonValue(int lessonId, int valueId, string value)
    {
        var lesson = await _db.Lectures.FirstOrDefaultAsync(x => x.Id == lessonId);
        if (lesson == null) throw new Exception("Lesson not found");
        switch (valueId)
        {
            case 1:
                lesson.Title = value;
                break;
            case 2:
                lesson.Teacher = value;
                break;
            case 3:
                lesson.LinkPlatform = value;
                break;
            case 4:
                lesson.Link = value;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(valueId), valueId, null);
        }

        await _db.SaveChangesAsync();
    }

    public async Task<Lecture?> GetLessonIn5Minutes()
    {
        var lecture = await GetLectureOnTime(TimeInUkraine);
        if (lecture is not null) return null;
        var lectureIn = await GetLectureOnTime(TimeInUkraine.AddMinutes(5));
        return lectureIn;
    }

    public async Task<bool> RegisterChat(long chatId)
    {
        var chat = await _db.ListenChats.FirstOrDefaultAsync(x => x.ChatId == chatId);
        if (chat != null) return false;
        await _db.ListenChats.AddAsync(new ListenChat
        {
            ChatId = chatId
        });
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnregisterChat(long chatId)
    {
        var chat = await _db.ListenChats.FirstOrDefaultAsync(x => x.ChatId == chatId);
        if (chat == null) return false;
        _db.ListenChats.Remove(chat);
        await _db.SaveChangesAsync();
        return true;
    }
}