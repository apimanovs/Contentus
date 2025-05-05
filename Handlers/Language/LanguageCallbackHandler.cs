using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu.Guide;

namespace TelegramStatsBot.Handlers.Language
{
    public class LanguageCallbackHandler : ICallbackHandler
    {
        public string Key => "lang_";

        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IMainMenuBuilder _menuBuilder;
        private readonly IMenuService _menuService;
        private readonly IGuideMenuBuilder _guideMenuBuilder;

        public LanguageCallbackHandler(
            IUserService userService,
            ITelegramBotClient bot,
            IMainMenuBuilder menuBuilder,
            IMenuService menuService,
            IGuideMenuBuilder guideMenuBuilder)
        {
            _userService = userService;
            _bot = bot;
            _menuBuilder = menuBuilder;
            _menuService = menuService;
            _guideMenuBuilder = guideMenuBuilder;
        }

        public async Task HandleAsync(CallbackQuery query)
        {
            var data = query.Data;
            var telegramId = query.From.Id;
            var chatId = query.Message.Chat.Id;

            string? language = data switch
            {
                "lang_ru" => "ru",
                "lang_en" => "en",
                "lang_ru_keep" => "ru",
                "lang_en_keep" => "en",
                _ => null
            };

            if (language == null)
            {
                await _bot.AnswerCallbackQueryAsync(query.Id, "❌ Invalid selection");
                return;
            }

            await _userService.SetUserLanguage(telegramId, language, true);

            string confirmationText = data.EndsWith("_keep")
                ? (language == "ru"
                    ? "🇷🇺 Отлично, продолжаем на <b>Русском</b>!"
                    : "🇬🇧 Great, continuing in <b>English</b>!")
                : (language == "ru"
                    ? "🇷🇺 Язык установлен на <b>Русский</b>"
                    : "🇬🇧 Language set to <b>English</b>");

            await _bot.EditMessageTextAsync(
                chatId: chatId,
                messageId: query.Message.MessageId,
                text: confirmationText,
                parseMode: ParseMode.Html
            );

            var user = await _userService.GetUserByTelegramIdAsync(telegramId);

            if (!user.HasSeenGuide)
            {
                var guideText = language == "ru"
                    ? "🧭 Хочешь пройти краткое обучение, чтобы понять как пользоваться ботом?"
                    : "🧭 Want to go through a short guide on how to use Teleboard?";

                var guideMenu = _guideMenuBuilder.GetGuideStartMenu(language);

                var lastMenuId = await _menuService.GetLastMenuMessageId(telegramId);
                
                if (lastMenuId != null)
                {
                    try
                    {
                        await _bot.EditMessageReplyMarkupAsync(chatId, lastMenuId.Value, null);
                        await _menuService.ClearLastMenuMessageId(telegramId);
                    }
                    catch { }
                }

                var sent = await _bot.SendTextMessageAsync(chatId, guideText, replyMarkup: guideMenu);

                await _menuService.SetLastMenuMessageId(telegramId, sent.MessageId);

                await _bot.AnswerCallbackQueryAsync(query.Id);
                return;
            }

            var menuText = language == "ru" ? "📋 Главное меню:" : "📋 Main menu:";
            var menu = _menuBuilder.GetMainMenu(language);

            var sentMenu = await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: menuText,
                replyMarkup: menu
            );

            await _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);
            await _bot.AnswerCallbackQueryAsync(query.Id);
        }
    }
}
