using Telegram.Bot.Types;

namespace TelegramStatsBot.Interfaces.Callback
{
    public interface ICallbackHandler
    {
        /// <summary>
        /// Callback data prefix that this handler should handle (e.g. "lang_").
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Handles the incoming callback query.
        /// </summary>
        Task HandleAsync(CallbackQuery query);
    }
}
