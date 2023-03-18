using System.Threading;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Khai518Bot.Bot.Commands.Entity
{
    [UsedImplicitly]
    [Command(@"anton")]
    public class Anton : Command
    {
        public override async Task Execute(Service service)
        {
            string description = "Вот ссылки на мой репозиторий и мой Telegram:";
            string repositoryLink = "https://github.com/Anton293/DICT_python_education_harkushyn_anton";
            string telegramLink = "https://t.me/Anton293";

            string messageText = $"{description}\n\n- [Репозиторий]({repositoryLink})\n- [Telegram]({telegramLink})";

            await BotClient.SendTextMessageAsync(Message.Chat, messageText, ParseMode.Markdown);
        }
    }
}
