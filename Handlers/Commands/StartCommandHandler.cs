using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Message;
using TelegramStatsBot.Interfaces.User;

namespace TelegramStatsBot.Handlers.Commands
{
    public class StartCommandHandler : IMessageHandler
    {
        public string Command => "/start";

        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IMainMenuBuilder _menuBuilder;
        private readonly IMenuService _menuService;

        public StartCommandHandler(
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

        public async Task HandleAsync(Message message)
        {
            var telegramId = message.From.Id;
            var chatId = message.Chat.Id;

            var user = await _userService.GetUserByTelegramIdAsync(telegramId);

            if (user == null)
            {
                var welcomeText = message.From.LanguageCode?.ToLower() == "ru"
                                ? "👋 Привет!"
                                : "👋 Hello!";

                await _bot.SendTextMessageAsync(
                    chatId: chatId,
                    text: welcomeText,
                    parseMode: ParseMode.Html
                );
            }

             user = await _userService.RegisterUserAsync(
                telegramId: telegramId,
                chatId: chatId,
                username: message.From.Username
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
                catch { }
            }

            if (string.IsNullOrEmpty(user.Language) || !user.IsLanguageConfirmed)
            {
                var systemLang = message.From.LanguageCode?.ToLower() ?? "en";
                var detectedLang = systemLang.StartsWith("ru") ? "ru" : "en";

                await _userService.SetUserLanguage(telegramId, detectedLang);

                var text = detectedLang == "ru"
                    ? "👨🏽‍💼 Я помогу тебе отслеживать рост подписчиков, охваты постов и активность аудитории прямо в Telegram.\r\nЯ заметил, что ты говоришь на <b>Русском</b>.\r\nОставим этот язык или выберем английский? 🌍"
                    : "👨🏽‍💼 I help you track subscriber growth, post reach and engagement — all inside Telegram.\r\nLooks like you're using <b>English</b>.\r\nShall we keep this language or switch to Russian? 🌍";

                var keyboard = new InlineKeyboardMarkup(new[]
                {
                    new[]
                    {
                        InlineKeyboardButton.WithCallbackData(
                            detectedLang == "ru" ? "✅ Оставить Русский" : "✅ Keep English",
                            $"lang_{detectedLang}_keep"
                        ),
                        InlineKeyboardButton.WithCallbackData(
                            detectedLang == "ru" ? "🌐 Switch to English" : "🌐 Переключиться на Русский",
                            detectedLang == "ru" ? "lang_en" : "lang_ru"
                        )
                    }
                });

                var sent = await _bot.SendTextMessageAsync(
                    chatId: chatId,
                    text: text,
                    parseMode: ParseMode.Html,
                    replyMarkup: keyboard
                );

                await _menuService.SetLastMenuMessageId(telegramId, sent.MessageId);
                return;
            }

            var menuText = user.Language == "ru" ? "📋 Главное меню:" : "📋 Main menu:";
            var menu = _menuBuilder.GetMainMenu(user.Language);

            var sentMenu = await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: menuText,
                replyMarkup: menu
            );

            await _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);
        }
    }
}
