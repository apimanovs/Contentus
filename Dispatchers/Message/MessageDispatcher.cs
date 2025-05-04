using TelegramStatsBot.Interfaces.Message;
using Telegram.Bot.Types;

namespace TelegramStatsBot.Dispatchers.Message
{
    public class MessageDispatcher
    {
        private readonly IEnumerable<IMessageHandler> _handlers;

        public MessageDispatcher(IEnumerable<IMessageHandler> handlers)
        {
            _handlers = handlers;
        }

        public async Task DispatchAsync(Update update)
        {
            var msg = update.Message;
            if (msg?.Text == null) return;

            var handler = _handlers.FirstOrDefault(h => h.Command == msg.Text);
            if (handler != null)
            {
                await handler.HandleAsync(msg);
            }
        }
    }
}
