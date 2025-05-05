using TelegramStatsBot.Interfaces.Menu.Guide;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace TelegramStatsBot.Builders.Menu.Guide
{
    public class GuideMenuBuilder : IGuideMenuBuilder
    {
        private readonly ITelegramBotClient _bot;

        public GuideMenuBuilder(ITelegramBotClient bot)
        {
            _bot = bot;
        }

        public InlineKeyboardMarkup GetGuideStartMenu(string language)
        {
            var isRu = language == "ru";

            return new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        isRu ? "✅ Да" : "✅ Yes",
                        "guide_step:1"
                    ),
                    InlineKeyboardButton.WithCallbackData(
                        isRu ? "❌ Нет" : "❌ No",
                        "guide_skip"
                    )
                }
            });
        }

        public InlineKeyboardMarkup GetStepKeyboard(int step, string language)
        {
            var isRu = language == "ru";
            var buttons = new List<InlineKeyboardButton[]>();

            if (step < 4)
            {
                buttons.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        isRu ? "⏭ Далее" : "⏭ Next",
                        $"guide_step:{step + 1}"
                    )
                });
            }

            if (step > 1)
            {
                buttons.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        isRu ? "⬅ Назад" : "⬅ Back",
                        $"guide_step:{step - 1}"
                    )
                });
            }

            if (step == 4)
            {
                buttons.Add(new[]
                {
                    InlineKeyboardButton.WithCallbackData(
                        isRu ? "📋 Перейти в меню" : "📋 Go to menu",
                        "open_main_menu"
                    )
                });
            }

            return new InlineKeyboardMarkup(buttons);
        }
    }
}
