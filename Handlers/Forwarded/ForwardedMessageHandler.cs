using Telegram.Bot;
using TelegramContentusBot.Interfaces.Forwarded.Channel;
using TelegramStatsBot.Interfaces.Handler;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Texsts.Menu;
using Telegram.Bot.Types.Enums;
using System.Globalization;

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

            user.CurrentStep = Enums.Onboarding.OnboardingStep.None;

            user.ChannelDetailsStep = TelegramContentusBot.Enums.ChannelDetails.ChannelDetailsSteps.About;

            var askAboutChannelText = user.Language == "ru"
                ? "📝 Расскажи немного о своём канале. Например, о чём он, какую цель ты преследуешь и для кого он предназначен."
                : "📝 Tell us a bit about your channel. For example, what is it about, what's your goal, and who is your target audience?";

            var sent = await _bot.SendTextMessageAsync(
                chatId: message.Chat.Id,
                text: askAboutChannelText,
                parseMode: ParseMode.Html
            );

            await _menuService.SetLastMenuMessageId(user.TelegramId, sent.MessageId);

        }
    }
}