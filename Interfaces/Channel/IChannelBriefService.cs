using TelegramStatsBot.Models.Result;

namespace TelegramContentusBot.Interfaces.Channel
{
    public interface IChannelBriefService
    {
        public Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveChannelAbouAsync(int channelId, int userId, string about);
    }
}
