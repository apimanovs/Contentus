using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramStatsBot.Interfaces.Menu.Main
{
    public interface IMainMenuBuilder
    {
        InlineKeyboardMarkup GetMainMenu(string language);
    }
}
