using Khai518Bot.Bot.Commands;
using Khai518Bot.Bot.Commands.Entity;
using Khai518Bot.Bot.Handler;
using Khai518Bot.Bot.Settings;
using Khai518Bot.Time;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSystemd();
builder.Services.AddLogging(p => p.AddConsole());

builder.Services.AddSingleton<IBotSettings, EnvBotSettings>();
builder.Services.AddSingleton<ITimeProvider, TimeProvider>();
builder.Services.AddDbContext<BotDbContext>((options, optionsBuilder) =>
    optionsBuilder.UseSqlite(options.GetRequiredService<IBotSettings>().DbConnection), ServiceLifetime.Singleton, ServiceLifetime.Singleton);
builder.Services.AddSingleton<ITelegramBotClient>(
    e => new TelegramBotClient(e.GetRequiredService<IBotSettings>().Token));
builder.Services.AddSingleton<ICommandFactory, CommandFactory>();
builder.Services.AddScoped<IBotHandler, BotHandler>();
builder.Services.AddSingleton<Service>();
builder.Services.AddHostedService<Webhook>();
builder.Services.AddHostedService<LessonsListen>();
builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddEndpointsApiExplorer();
var app = builder.Build();
app.MapControllers();
app.Run();