using TelegramStatsBot.Interfaces.Callback;
using Telegram.Bot.Types;

namespace TelegramStatsBot.Dispatchers.Callback
{
    public class CallbackDispatcher
    {
        private readonly IEnumerable<ICallbackHandler> _handlers;

        public CallbackDispatcher(IEnumerable<ICallbackHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task DispatchAsync(Update update)
        {
            var query = update.CallbackQuery;
            if (query == null || string.IsNullOrEmpty(query.Data)) return;

            var handler = _handlers.FirstOrDefault(h => query.Data.StartsWith(h.Key));
            if (handler != null)
            {
                await handler.HandleAsync(query);
            }
        }
    }
}
