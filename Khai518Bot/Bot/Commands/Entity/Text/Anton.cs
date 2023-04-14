using Khai518Bot.Bot.Commands.Attributes;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity.Text
{
    [TextCommand(@"anton", @"Информация об антоне", 1)]
    public class Anton : Command
    {
        public override async Task Execute(Service service)
        {
            // Разбиение текста сообщения на аргументы
            string[] args = Message.Text.Split(' ');
            if (args.Length > 1)
            {
                if (args[1] == "info")
                {
                    // Описание и ссылки
                    string description = "Вот ссылки на мой репозиторий и мой Telegram:";
                    string repositoryLink = "https://github.com/Anton293/DICT_python_education_harkushyn_anton";
                    string telegramLink = "https://t.me/Anton293";

                    // Формирование текста сообщения с использованием Markdown
                    string messageText = $"{description}\n\n- [Репозиторий]({repositoryLink})\n- [Telegram]({telegramLink})";

                    // Отправка сообщения
                    await BotClient.SendTextMessageAsync(Message.Chat, messageText, ParseMode.Markdown);
                }
                else if (args[1] == "help")
                {
                    string messageText = $"Доступный аргумент: info";

                    // Отправка сообщения
                    await BotClient.SendTextMessageAsync(Message.Chat, messageText, ParseMode.Markdown);
                }
            }
            else
            {
                string messageText = $"/anton info\nИспользуйте команду /anton help для получения списка доступных аргументов";

                // Отправка сообщения
                await BotClient.SendTextMessageAsync(Message.Chat, messageText, ParseMode.Markdown);
            }
        }
    }
}
