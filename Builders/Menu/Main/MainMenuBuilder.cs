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

        public InlineKeyboardMarkup GetMainMenu(string language, bool hasChannels)
        {
            if (!hasChannels)
            {
                return new InlineKeyboardMarkup(new[]
                {
                    new[] {
                        InlineKeyboardButton.WithCallbackData(
                            language == "ru" ? "➕ Добавить канал" : "➕ Add Channel",
                            "add_channel:start")
                    }
                });
            }

            return new InlineKeyboardMarkup(new[]
            {
                new[] {
                    InlineKeyboardButton.WithCallbackData(
                        language == "ru" ? "📝 Генерация поста" : "📝 Generate Post",
                        "gen_post")
                },
                new[] {
                    InlineKeyboardButton.WithCallbackData(
                        language == "ru" ? "⚙️ Мои каналы" : "⚙️ My Channels",
                        "my_channels")
                }
            });
        }
    }
}
