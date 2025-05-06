using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using TelegramStatsBot.Database;
using TelegramStatsBot.Interfaces.Channel;
using TelegramStatsBot.Models.Result;

namespace TelegramStatsBot.Services.Channel
{
    public class ChannelService : IChannelService
    {
        private readonly ITelegramBotClient _bot;
        private readonly DataContext _context;

        public ChannelService (ITelegramBotClient bot, DataContext dataContext)
        {
            _bot = bot;
            _context = dataContext;
        }

        public async Task<OperationResult<Models.Channel.Channel>> AddChannelAsync(string link, int userId)
        {
            if (string.IsNullOrWhiteSpace(link))
                return OperationResult<Models.Channel.Channel>.Fail("⚠️ Ссылка не может быть пустой.");

            var exists = await _context.Channels
                .FirstOrDefaultAsync(c => c.ChannelLink == link && c.UserId == userId);

            if (exists != null)
                return OperationResult<Models.Channel.Channel>.Fail("📌 Ты уже добавил этот канал.");

            var channel = new Models.Channel.Channel
            {
                UserId = userId,
                ChannelLink = link,
                LinkedAt = DateTime.UtcNow
            };

            _context.Channels.Add(channel);
            await _context.SaveChangesAsync();

            return OperationResult<Models.Channel.Channel>.Ok(channel);
        }
    }
}
