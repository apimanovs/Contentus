using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramStatsBot.Interfaces.Menu.Guide
{
    public interface IGuideMenuBuilder
    {
        InlineKeyboardMarkup GetStepKeyboard(int step, string language);
        InlineKeyboardMarkup GetGuideStartMenu(string language);
    }
}
