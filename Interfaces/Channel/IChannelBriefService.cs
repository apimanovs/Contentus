using TelegramStatsBot.Models.Result;

namespace TelegramContentusBot.Interfaces.Channel
{
    public interface IChannelBriefService
    {
        Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveChannelAbouAsync(int channelId, int userId, string about);

        Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveTargetAudienceAsync(int channelId, int userId, string targetAudience);

        Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SavePreferedStyleAsync(int userId, int channelId, string preferedStyle);

        Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveContentGoalAsync(int userId, int channelId, string contentGoal);

        Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveExamplePostAsync(int userId, int channelId, string examplePost);
    }
}
