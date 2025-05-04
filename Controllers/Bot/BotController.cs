using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Dispatchers.Callback;
using TelegramStatsBot.Dispatchers.Message;

namespace TelegramStatsBot.Controllers.Bot
{
    [ApiController]
    [Route("webhook")]
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MessageDispatcher _messageDispatcher;
        private readonly CallbackDispatcher _callbackDispatcher;

        public BotController(ITelegramBotClient botClient, MessageDispatcher messageDispatcher, CallbackDispatcher callbackDispatcher)
        {
            _botClient = botClient;
            _messageDispatcher = messageDispatcher;
            _callbackDispatcher = callbackDispatcher;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
            try
            {
                if (update.Type == UpdateType.Message && update.Message?.Text != null)
                {
                    await _messageDispatcher.DispatchAsync(update);
                }
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    await _callbackDispatcher.DispatchAsync(update);
                }

                Console.WriteLine($"[Webhook] Update type: {update.Type}");

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine("🔥 Webhook error: " + ex.Message);
                return Ok();
            }
        }
    }
}