using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Message;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu.Guide;
using TelegramStatsBot.Enums.Onboarding;
using Telegram.Bot.Requests;
using TelegramStatsBot.Texsts.Menu;

namespace TelegramStatsBot.Handlers.Commands
{
    public class StartCommandHandler : IMessageHandler
    {
        public string Command => "/start";

        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IMainMenuBuilder _menuBuilder;
        private readonly IMenuService _menuService;
        private readonly IGuideMenuBuilder _guideMenuBuilder;

        public StartCommandHandler(
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
                      ? "🧠 Привет! Я — Contentus, ИИ-копирайтер для Telegram-каналов.\r\nМоя работа — помогать тебе создавать посты, придумывать заголовки, улучшать тексты и заполнять контент-план.\r\nЯ могу предложить варианты постов по теме, сократить длинный текст, добавить call-to-action или превратить сухой текст в живой и вовлекающий.\r\n\r\nТы используешь <b>Русский</b>. Оставим его или переключимся на English? 🌍"
                      : "🧠 Hi! I'm Contentus — an AI copywriter for Telegram channels.\r\nMy job is to help you write posts, generate headlines, improve text, and fill your content calendar.\r\nI can suggest post ideas, shorten long text, add calls to action, or rewrite something to make it more engaging.\r\n\r\nLooks like you're using <b>English</b>. Shall we keep it or switch to Russian? 🌍";


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

            if (user.HasSeenGuide == false)
            {
                var guideText = user.Language == "ru"
                     ? "🧭 Хочешь пройти краткое обучение, чтобы понять как пользоваться ботом?"
                     : "🧭 Want to go through a short guide on how to use Contentus?";

                var guideMenu = _guideMenuBuilder.GetGuideStartMenu(user.Language);

                var sent = await _bot.SendTextMessageAsync(
                     chatId: chatId,
                     text: guideText,
                     parseMode: ParseMode.Html,
                     replyMarkup: guideMenu
                );

                await _menuService.SetLastMenuMessageId(telegramId, sent.MessageId);

                return;
            }

            var menuText = MenuTexts.GetMainMenuTitle(user.Language);

            var hasChannels = await _userService.HasAnyChannels(user.Id);
            var menu = _menuBuilder.GetMainMenu(user.Language, hasChannels);

             var sentMenu = await _bot.SendTextMessageAsync(
                 chatId: chatId,
                 text: menuText,
                 replyMarkup: menu
             );

             await _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);
        }
    }
}
