using TelegramStatsBot.Interfaces.Message;
using Telegram.Bot.Types;
using System.Threading.Channels;
using Telegram.Bot.Types.Enums;
using TelegramStatsBot.Interfaces.Forward;
using TelegramStatsBot.Interfaces.Forward.Handler;

namespace TelegramStatsBot.Dispatchers.Message
{
    public class MessageDispatcher
    {
        private readonly IEnumerable<IMessageHandler> _handlers;
        private readonly IEnumerable<IForwardedMessageHandler> _forwardHandlers;

        public MessageDispatcher(
            IEnumerable<IMessageHandler> handlers,
            IEnumerable<IForwardedMessageHandler> forwardHandlers)
        {
            _handlers = handlers;
            _forwardHandlers = forwardHandlers;
        }

        public async Task DispatchAsync(Update update)
        {
            var msg = update.Message;
            if (msg == null) return;

            if (msg.ForwardFromChat?.Type == ChatType.Channel)
            {
                foreach (var forwardHandler in _forwardHandlers)
                {
                    await forwardHandler.HandleForwardedAsync(msg);
                }
                return;
            }

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
