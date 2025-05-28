using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramStatsBot.Interfaces.Forward;
using TelegramStatsBot.Interfaces.Forward.Handler;
using TelegramStatsBot.Interfaces.Forward.Service;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Models.Result;
using TelegramStatsBot.Texsts.Menu;

namespace TelegramStatsBot.Handlers.Forwarded
{
    public class ForwardedMessageHandler : IForwardedMessageHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly IForwardChannelMessageService _channelMessageService;
        private readonly IUserService _userService;
        private readonly IMainMenuBuilder _mainMenuBuilder;
        private readonly IMenuService _menuService;

        public ForwardedMessageHandler(
            ITelegramBotClient bot,
            IForwardChannelMessageService service,
            IUserService userService, IMainMenuBuilder mainMenuBuilder, IMenuService menuService)
        {
            _bot = bot;
            _channelMessageService = service;
            _userService = userService;
            _mainMenuBuilder = mainMenuBuilder;
            _menuService = menuService;
        }


        public async Task HandleForwardedAsync(Telegram.Bot.Types.Message message)
        {
            var user = await _userService.GetUserByTelegramIdAsync(message.From.Id);

            var result = await _channelMessageService.ProcessForwardedChannelAsync(
                message: message,
                userId: user.Id
            );

            if (!result.Success)
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, result.Error);
                return;
            }

            string? channelTitle = message.ForwardFromChat.Title;

            var successText = user.Language == "ru"
                ? $"🎉 Канал {channelTitle} успешно добавлен по пересланному сообщению!"
                : $"🎉 Channel {channelTitle} successfully linked via forwarded message!";

            await _bot.SendTextMessageAsync(message.Chat.Id, successText);


            var keyboard = _mainMenuBuilder.GetMainMenu(user.Language);
            var menuText = MenuTexts.GetMainMenuTitle(user.Language);

            var sent = await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: menuText,
                replyMarkup: keyboard
            );

            await _menuService.SetLastMenuMessageId(user.TelegramId, sent.MessageId);

        }
    }
}
