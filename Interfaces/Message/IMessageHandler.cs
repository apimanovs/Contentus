using Telegram.Bot.Types;

namespace TelegramStatsBot.Interfaces.Message
{
    public interface IMessageHandler
    {
        string Command { get; }
        Task HandleAsync(Telegram.Bot.Types.Message message);
    }
}
