using TelegramStatsBot.Interfaces.Message;
using Telegram.Bot.Types;
using System.Threading.Channels;
using Telegram.Bot.Types.Enums;


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
            if (msg == null) return;

            if (msg.Text == null) return;

            var handler = _handlers.FirstOrDefault(h => h.Command == msg.Text)
                ?? _handlers.FirstOrDefault(h => string.IsNullOrWhiteSpace(h.Command));

            if (handler != null)
            {
                await handler.HandleAsync(msg);
            }
        }
    }

}
