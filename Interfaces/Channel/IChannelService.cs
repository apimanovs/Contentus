using TelegramStatsBot.Models.Result;

namespace TelegramStatsBot.Interfaces.Channel
{
    public interface IChannelService
    {
        Task<OperationResult<Models.Channel.Channel>> AddChannelAsync(string link, int userId);
    }
}
