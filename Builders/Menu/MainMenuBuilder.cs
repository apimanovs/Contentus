using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;

namespace TelegramStatsBot.Builders.Menu
{
    public class MainMenuBuilder : IMainMenuBuilder
    {
        private readonly ITelegramBotClient _bot;
        private readonly IMenuService _menuService;

        public MainMenuBuilder(ITelegramBotClient bot, IMenuService menuService)
        {
            _bot = bot;
            _menuService = menuService;
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
