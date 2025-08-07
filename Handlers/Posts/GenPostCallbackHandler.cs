using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.User;
using TelegramContentusBot.Requests.Posts;

namespace TelegramContentusBot.Handlers.Posts
{
    public class GenPostCallbackHandler : ICallbackHandler
    {
        public string Key => "gen_post";

        private readonly ITelegramBotClient _bot;
        private readonly IUserService _userService;
        private readonly PostGenerationRequest _postRequest;

        public GenPostCallbackHandler(
            ITelegramBotClient bot,
            IUserService userService,
            PostGenerationRequest postRequest)
        {
            _bot = bot;
            _userService = userService;
            _postRequest = postRequest;
        }

        public async Task HandleAsync(CallbackQuery query)
        {
            var telegramId = query.From.Id;
            var chatId = query.Message.Chat.Id;

            var user = await _userService.GetUserByTelegramIdAsync(telegramId);
            if (user == null)
            {
                await _bot.AnswerCallbackQueryAsync(query.Id, "❌ Пользователь не найден");
                return;
            }

            await _bot.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.Typing);

            var result = await _postRequest.GeneratePostAsync(user.Id);

            await _bot.SendTextMessageAsync(
                chatId: chatId,
                text: result,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Html
            );
        }
    }
}
