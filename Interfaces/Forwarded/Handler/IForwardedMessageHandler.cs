namespace TelegramStatsBot.Interfaces.Handler
{
    public interface IForwardedMessageHandler
    {
        Task HandleForwardedAsync(Telegram.Bot.Types.Message message);
    }
}