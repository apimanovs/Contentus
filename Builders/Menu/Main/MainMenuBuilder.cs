using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;

namespace TelegramStatsBot.Builders.Menu
{
    public class MainMenuBuilder : IMainMenuBuilder
    {
        private readonly ITelegramBotClient _bot;

        public MainMenuBuilder(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public InlineKeyboardMarkup GetMainMenu(string language)
        {
            var isRu = language == "ru";

            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(isRu ? "➕ Добавить" : "➕ Add", "menu_add"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(isRu ? "📊 Отчёт" : "📊 Report", "menu_report")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("➕ Добавить счёт", "account_create")
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(isRu ? "⚙️ Настройки" : "⚙️ Settings", "menu_settings")
                }
            });
        }
    }
}
