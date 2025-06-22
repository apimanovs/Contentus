using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Message;
using TelegramContentusBot.Enums.ChannelDetails;
using TelegramContentusBot.Interfaces.Channel;

namespace TelegramContentusBot.Handlers.Channels.Details
{
    public class ChannelBriefHandler : IMessageHandler
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;
        private readonly IChannelBriefService _channelBriefService;
        // TODO: Добавь другие сервисы (например, ITargetAudienceService, IStyleService)

        public ChannelBriefHandler(
            IUserService userService,
            ITelegramBotClient bot,
            IChannelBriefService channelBriefService
        // TODO: Добавь в конструктор остальные сервисы
        )
        {
            _userService = userService;
            _bot = bot;
            _channelBriefService = channelBriefService;
        }

        public string Command => "";

        public async Task HandleAsync(Message message)
        {
            var user = await _userService.GetUserByTelegramIdAsync(message.From.Id);
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

                    user.ChannelDetailsStep = ChannelDetailsSteps.TargetAudience;
                    await _bot.SendTextMessageAsync(chatId, "✅ Отлично! 👥 Кто твоя целевая аудитория?");
                    break;

                case ChannelDetailsSteps.TargetAudience:
                    // TODO: Вставь вызов SaveTargetAudienceAsync
                    user.ChannelDetailsStep = ChannelDetailsSteps.StylePreference;
                    await _bot.SendTextMessageAsync(chatId, "✅ Отлично! 🎨 Какой стиль ты предпочитаешь для канала?");
                    break;

                case ChannelDetailsSteps.StylePreference:
                    // TODO: Вставь вызов SaveStylePreferenceAsync
                    user.ChannelDetailsStep = ChannelDetailsSteps.ContentGoal;
                    await _bot.SendTextMessageAsync(chatId, "✅ Отлично! Цель твоего контента.");
                    break; 
                
                case ChannelDetailsSteps.ContentGoal:
                    // TODO: Вставь вызов SaveContentGoalAsync
                    user.ChannelDetailsStep = ChannelDetailsSteps.ExamplePost;
                    await _bot.SendTextMessageAsync(chatId, "✅ Отлично! Пример поста");
                    break;
                
                case ChannelDetailsSteps.ExamplePost:
                    // TODO: Вставь вызов SaveContentGoalAsync
                    user.ChannelDetailsStep = ChannelDetailsSteps.Done;
                    await _bot.SendTextMessageAsync(chatId, "✅ Отлично! Ты успешно заполнил данные о канале.");
                    break;

                default:
                    await _bot.SendTextMessageAsync(chatId, "⚠️ Неизвестный этап заполнения.");
                    break;
            }
        }
    }
}
