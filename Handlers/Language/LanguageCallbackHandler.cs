using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.User;

namespace TelegramStatsBot.Handlers.Language
{
    public class LanguageCallbackHandler : ICallbackHandler
    {
        public string Key => "lang_";

        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IMainMenuBuilder _menuBuilder;
        private readonly IMenuService _menuService;

        public LanguageCallbackHandler(
            IUserService userService,
            ITelegramBotClient bot,
            IMainMenuBuilder menuBuilder,
            IMenuService menuService)
        {
            _userService = userService;
            _bot = bot;
            _menuBuilder = menuBuilder;
            _menuService = menuService;
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

            string confirmationText;
            if (data.EndsWith("_keep"))
            {
                confirmationText = language == "ru"
                    ? "🇷🇺 Отлично, продолжаем на <b>Русском</b>!"
                    : "🇬🇧 Great, continuing in <b>English</b>!";
            }
            else
            {
                confirmationText = language == "ru"
                    ? "🇷🇺 Язык установлен на <b>Русский</b>"
                    : "🇬🇧 Language set to <b>English</b>";
            }

            await _bot.EditMessageTextAsync(
                chatId: chatId,
                messageId: query.Message.MessageId,
                text: confirmationText,
                parseMode: ParseMode.Html
            );

            var lastMenuId = await _menuService.GetLastMenuMessageId(telegramId);
            if (lastMenuId != null)
            {
                try
                {
                    await _bot.EditMessageReplyMarkupAsync(
                        chatId: chatId,
                        messageId: lastMenuId.Value,
                        replyMarkup: null
                    );
                    await _menuService.ClearLastMenuMessageId(telegramId);
                }
                catch
                {
                }
            }

            var menuText = language == "ru" ? "📋 Главное меню:" : "📋 Main menu:";
            var menu = _menuBuilder.GetMainMenu(language);

            var sentMenu = await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: menuText,
                replyMarkup: menu
            );

            _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);

            await _bot.AnswerCallbackQueryAsync(query.Id);
        }
    }
}
