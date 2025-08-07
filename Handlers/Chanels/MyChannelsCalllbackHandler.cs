using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Texsts.Menu;

namespace TelegramContentusBot.Handlers.Chanels
{
    public class MyChannelsCalllbackHandler : ICallbackHandler
    {
        public string Key => "my_channels";

        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IMenuService _menuService;
        private readonly IMainMenuBuilder _mainMenuBuilder;

        public MyChannelsCalllbackHandler(
            IUserService userService,
            ITelegramBotClient bot,
            IMenuService menuService,
            IMainMenuBuilder mainMenuBuilder)
        {
            _userService = userService;
            _bot = bot;
            _menuService = menuService;
            _mainMenuBuilder = mainMenuBuilder;
        }

        public async Task HandleAsync(CallbackQuery query)
        {
            var telegramId = query.From.Id;
            var chatId = query.Message.Chat.Id;

            var user = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (user == null)
            {
                await _bot.AnswerCallbackQueryAsync(query.Id, "❌ User not found");
                return;
            }

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

            var channels = await _userService.GetUserChannelsById(user.Id);
            if (channels.Count == 0)
            {
                await _bot.SendTextMessageAsync(chatId, "⚠️ У вас нет каналов. Пожалуйста, добавьте канал.");
                return;
            }

            if (channels.Count == 0)
            {
                await _bot.SendTextMessageAsync(chatId, "⚠️ У вас нет каналов. Пожалуйста, добавьте канал.");
                return;
            }

            var channelList = string.Join("\n\n", channels.Select(c =>
                $"📣 <b>{c.ChannelTitle}</b> {(string.IsNullOrEmpty(c.ChannelUsername) ? "" : $"(@{c.ChannelUsername})")}\n" +
                $"🧾 <b>Описание:</b> {(string.IsNullOrEmpty(c.About) ? "—" : c.About)}\n" +
                $"🎯 <b>Целевая аудитория:</b> {(string.IsNullOrEmpty(c.TargetAudience) ? "—" : c.TargetAudience)}\n" +
                $"🎯 <b>Цель контента:</b> {(string.IsNullOrEmpty(c.ContentGoal) ? "—" : c.ContentGoal)}\n" +
                $"🎨 <b>Стиль постов:</b> {(string.IsNullOrEmpty(c.StylePreference) ? "—" : c.StylePreference)}\n" +
                $"🕐 <b>Привязан:</b> {c.LinkedAt:dd.MM.yyyy}")
            );

            await _bot.SendTextMessageAsync(
                chatId,
                $"<b>Ваш канал:</b>\n\n{channelList}",
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
            );

            var hasChannels = await _userService.HasAnyChannels(user.Id);

            var mainMenu = _mainMenuBuilder.GetMainMenu(user.Language, true);

            var menuTitle = MenuTexts.GetMainMenuTitle(user.Language, hasChannels);

            var sentMenu = await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: menuTitle,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html,
                replyMarkup: mainMenu
            );

            await _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);
        }

    }
}
