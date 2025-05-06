using Telegram.Bot;
using TelegramStatsBot.Interfaces.Forward;
using TelegramStatsBot.Interfaces.Forward.Handler;
using TelegramStatsBot.Interfaces.Forward.Service;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Models.Result;

namespace TelegramStatsBot.Handlers.Forwarded
{
    public class ForwardedMessageHandler : IForwardedMessageHandler
    {
        private readonly ITelegramBotClient _bot;
        private readonly IForwardChannelMessageService _channelMessageService;
        private readonly IUserService _userService;

        public ForwardedMessageHandler(
            ITelegramBotClient bot,
            IForwardChannelMessageService service,
            IUserService userService)
        {
            _bot = bot;
            _channelMessageService = service;
            _userService = userService;
        }


        public async Task HandleForwardedAsync(Telegram.Bot.Types.Message message)
        {
            var user = await _userService.GetUserByTelegramIdAsync(message.From.Id);

            await _bot.SendTextMessageAsync(message.Chat.Id, "sadasdasdasdasd");


            var result = await _channelMessageService.ProcessForwardedChannelAsync(
                message: message,
                userId: user.Id
            );

            await _bot.SendTextMessageAsync(message.Chat.Id, "sadasdasdasdasd");


            if (!result.Success)
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, result.Error);
                return;
            }

            var successText = user.Language == "ru"
                ? "🎉 Канал успешно добавлен по пересланному сообщению!"
                : "🎉 Channel successfully linked via forwarded message!";

            await _bot.SendTextMessageAsync(message.Chat.Id, successText);
        }
    }
}
