using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace TelegramStatsBot.Controllers.Bot
{
    [ApiController]
    [Route("webhook")]
    public class BotController : ControllerBase
    {
        private readonly ITelegramBotClient _botClient;

        public BotController(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Update update)
        {
           await _botClient.SendTextMessageAsync(
                chatId: update.Message.Chat.Id,
                text: "Hello, World!",
                parseMode: ParseMode.Markdown
            );

            return Ok();
        }
    }
}
