using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramStatsBot.Models.Result;
using TelegramStatsBot.Models.Channel;

namespace TelegramStatsBot.Interfaces.Forward.Service
{
    public interface IForwardChannelMessageService
    {
        Task<OperationResult<Models.Channel.Channel>> ProcessForwardedChannelAsync(Telegram.Bot.Types.Message message, int userId);
    }
}
