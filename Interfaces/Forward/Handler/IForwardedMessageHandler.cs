namespace TelegramStatsBot.Interfaces.Forward.Handler
{
    public interface IForwardedMessageHandler
    {
        Task HandleForwardedAsync(Telegram.Bot.Types.Message message);
    }
}
