using System.Threading.Tasks;
using Telegram.Bot.Types;
using TelegramStatsBot.Models.Result;
using TelegramStatsBot.Models.Channel;

namespace TelegramContentusBot.Interfaces.Forwarded.Channel
{
    public interface IForwardChannelMessageService
    {
        Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> ProcessForwardedChannelAsync(Message message, int userId);
    }
}