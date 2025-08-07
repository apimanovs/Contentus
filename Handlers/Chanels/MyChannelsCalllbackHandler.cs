using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Menu.Main;
using Telegram.Bot;
using Telegram.Bot.Types;

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

            string channelList = string.Join("\n", channels.Select(c => $"{c.ChannelTitle} - {c.Id}"));
            await _bot.SendTextMessageAsync(chatId, $"Ваши каналы:\n{channelList}");

            var mainMenu = _mainMenuBuilder.GetMainMenu(user.Language, true);
            var sentMenu = await _bot.SendTextMessageAsync(chatId, "Выберите действие:", replyMarkup: mainMenu);

            await _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);
        }

    }
}
