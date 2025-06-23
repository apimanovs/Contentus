using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Message;
using TelegramContentusBot.Enums.ChannelDetails;
using TelegramContentusBot.Interfaces.Channel;
using TelegramStatsBot.Texsts.Menu;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;

namespace TelegramContentusBot.Handlers.Channels.Details
{
    public class ChannelBriefHandler : IMessageHandler
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IChannelBriefService _channelBriefService;
        private readonly IMainMenuBuilder _mainMenuBuilder;
        private readonly IMenuService _menuService;

        public ChannelBriefHandler(
            IUserService userService,
            ITelegramBotClient bot,
            IChannelBriefService channelBriefService,
            IMainMenuBuilder mainMenuBuilder,
            IMenuService menuService
        )
        {
            _userService = userService;
            _bot = bot;
            _channelBriefService = channelBriefService;
            _mainMenuBuilder = mainMenuBuilder;
            _menuService = menuService;
        }

        public string Command => "";

        public async Task HandleAsync(Message message)
        {
            try
            { 
                var user = await _userService.GetUserByTelegramIdAsync(message.From.Id);
                var telegramId = message.From.Id;

                if (user == null || user.ChannelDetailsStep is ChannelDetailsSteps.None or ChannelDetailsSteps.Done)
                    return;

                var chatId = message.Chat.Id;
                var channelId = user.LastEditedChannelId;

                if (channelId == null)
                {
                    await _bot.SendTextMessageAsync(chatId, "⚠️ Не удалось определить канал. Пожалуйста, начни сначала.");
                    return;
                }

                switch (user.ChannelDetailsStep)
                {
                    case ChannelDetailsSteps.About:

                        var aboutResult = await _channelBriefService.SaveChannelAbouAsync(
                            channelId.Value,
                            user.Id,
                            message.Text.Trim());

                        if (!aboutResult.Success)
                        {
                            await _bot.SendTextMessageAsync(chatId, aboutResult.Error);
                            return;
                        }

                        await _bot.SendTextMessageAsync(chatId, "✅ Отлично! 👥 Кто твоя целевая аудитория?");
                        break;

                    case ChannelDetailsSteps.TargetAudience:
                        
                        var targetAudienceResult = await _channelBriefService.SaveTargetAudienceAsync(
                            channelId.Value,
                            user.Id,
                            message.Text.Trim());

                        if (!targetAudienceResult.Success)
                        {
                            await _bot.SendTextMessageAsync(chatId, targetAudienceResult.Error);
                            return;
                        }

                        await _bot.SendTextMessageAsync(chatId, "✅ Отлично! 🎨 Какой стиль ты предпочитаешь для канала?");
                        break;

                    case ChannelDetailsSteps.StylePreference:
                        var styleResult = await _channelBriefService.SavePreferedStyleAsync(
                            user.Id,
                            channelId.Value,
                            message.Text.Trim());

                        if (!styleResult.Success)
                        {
                            await _bot.SendTextMessageAsync(chatId, styleResult.Error);
                            return;
                        }

                        await _bot.SendTextMessageAsync(chatId, "✅ Отлично! Цель твоего контента.");
                        break;

                    case ChannelDetailsSteps.ContentGoal:
                        
                        var contentGoalResult = await _channelBriefService.SaveContentGoalAsync(
                            user.Id,
                            channelId.Value,
                            message.Text.Trim());

                        await _bot.SendTextMessageAsync(chatId, "✅ Отлично! Пример поста");
                        break;

                    case ChannelDetailsSteps.ExamplePost:
                        
                        var examplePostREsult = await _channelBriefService.SaveExamplePostAsync(user.Id, channelId.Value, message.Text.Trim());

                        if (!examplePostREsult.Success)
                        {
                            await _bot.SendTextMessageAsync(chatId, examplePostREsult.Error);
                            return;
                        }

                        await _bot.SendTextMessageAsync(chatId, "✅ Отлично! Ты успешно заполнил данные о канале.");

                        bool hasChannels = await _userService.HasAnyChannels(user.Id);

                        var menuText = MenuTexts.GetMainMenuTitle(user.Language, hasChannels);

                        var menu = _mainMenuBuilder.GetMainMenu(user.Language, hasChannels);

                        var sentMenu = await _bot.SendTextMessageAsync(
                            chatId: chatId,
                            text: menuText,
                            replyMarkup: menu
                        );

                        await _menuService.SetLastMenuMessageId(telegramId, sentMenu.MessageId);

                        break;

                    default:
                        await _bot.SendTextMessageAsync(chatId, "⚠️ Неизвестный этап заполнения.");
                        break;
                }
            }
            catch (Exception ex)
            {
                await _bot.SendTextMessageAsync(message.Chat.Id, $"⚠️ Ошибка: {ex.Message}");
                return;
            }
        }
    }
}
