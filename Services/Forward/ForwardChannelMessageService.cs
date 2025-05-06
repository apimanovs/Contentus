using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramStatsBot.Database;
using TelegramStatsBot.Interfaces.Forward.Service;
using TelegramStatsBot.Models.Result;

namespace TelegramStatsBot.Services.Forward
{
    public class ForwardChannelMessageService : IForwardChannelMessageService
    {
        private readonly ITelegramBotClient _bot;
        private readonly DataContext _context;
        public ForwardChannelMessageService(ITelegramBotClient bot, DataContext dataContext)
        {
            _bot = bot;
            _context = dataContext;
        }

        public async Task<OperationResult<Models.Channel.Channel>> ProcessForwardedChannelAsync(Telegram.Bot.Types.Message message, int userId)
        {
            if (message.ForwardFromChat == null || message.ForwardFromChat.Type != Telegram.Bot.Types.Enums.ChatType.Channel)
            {
                return OperationResult<Models.Channel.Channel>.Fail("⚠️ Это сообщение не переслано из канала");
            }

            var chat = message.ForwardFromChat;

            var exists = await _context.Channels.FirstOrDefaultAsync(c => c.ChannelId == chat.Id && c.UserId == message.From.Id);

            if (exists != null)
            {
                return OperationResult<Models.Channel.Channel>.Fail("📌 Ты уже добавил этот канал.");
            }

            var botInfo = await _bot.GetMeAsync();

            var admins = await _bot.GetChatAdministratorsAsync(chat.Id);

            var botAdmin = admins
                .OfType<Telegram.Bot.Types.ChatMemberAdministrator>()
                .FirstOrDefault(a => a.User.Id == botInfo.Id);

            if (botAdmin == null || botAdmin.IsAnonymous)
            {
                return OperationResult<Models.Channel.Channel>.Fail(
                    "⚠️ Я не админ в этом канале или админ, но с включённой анонимностью. " +
                    "Пожалуйста, убери флажок «Оставаться анонимным» в настройках администратора канала.");
            }

            var channel = new Models.Channel.Channel
            {
                UserId = userId,
                ChannelTitle = chat.Title,
                ChannelUsername = chat.Username,
                ChannelId = chat.Id,
                ChannelLink = chat.InviteLink,
                IsBotAdmin = true,
                LinkedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channel);

            await _context.SaveChangesAsync();

            return OperationResult<Models.Channel.Channel>.Ok(channel);
        }
    }
}
