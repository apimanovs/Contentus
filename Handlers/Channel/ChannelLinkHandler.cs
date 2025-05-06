using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Enums.Onboarding;
using TelegramStatsBot.Interfaces.Message;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Channel;

namespace TelegramStatsBot.Handlers.Channel
{
    public class ChannelLinkHandler : IMessageHandler
    {
        public string Command => "";

        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IChannelService _channelService;

        public ChannelLinkHandler(IUserService userService, ITelegramBotClient bot, IChannelService channelService)
        {
            _userService = userService;
            _bot = bot;
            _channelService = channelService;
        }

        public async Task HandleAsync(Message message)
        {
            var telegramId = message.From.Id;
            var chatId = message.Chat.Id;

            var user = await _userService.GetUserByTelegramIdAsync(telegramId);

            await _bot.SendTextMessageAsync(chatId, $"Current step: {user.CurrentStep}");

            if (user.CurrentStep != OnboardingStep.AwaitingChannelLink)
                return;


            var link = message.Text?.Trim();

            if (string.IsNullOrEmpty(link) ||
                (!link.StartsWith("https://t.me/") && !link.StartsWith("https://t.me/+")))
            {
                var retryText = user.Language == "ru"
                    ? "❗️Пожалуйста, пришли корректную ссылку на канал, например:\n\nhttps://t.me/yourchannel"
                    : "❗️Please send a valid channel link like:\n\nhttps://t.me/yourchannel";

                await _bot.SendTextMessageAsync(chatId, retryText);
                return;
            }

            var chat = await _bot.GetChatAsync(link);

            var result = await _channelService.AddChannelAsync(link, user.Id);

            if (!result.Success)
            {
                await _bot.SendTextMessageAsync(chatId, result.Error);
                return;
            }

            user.CurrentStep = OnboardingStep.None;
            await _userService.UpdateUserAsync(user);

            var successText = user.Language == "ru"
                ? "🎉 Отлично! Канал подключен. Теперь ты можешь перейти в главное меню."
                : "🎉 Great! Channel connected. Now you can go to the main menu.";

            await _bot.SendTextMessageAsync(chatId, successText);
        }
    }
}
