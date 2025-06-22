using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramStatsBot.Database;
using TelegramStatsBot.Interfaces;
using TelegramStatsBot.Models.Result;
using TelegramStatsBot.Interfaces.User;
using TelegramContentusBot.Interfaces.Forwarded.Channel;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TelegramContentusBot.Interfaces.Channel;

namespace TelegramContentusBot.Services.Channel
{
    public class ChannelBriefService : IChannelBriefService
    {
        private readonly ITelegramBotClient _bot;
        private readonly IUserService _userService;
        private readonly DataContext _context;

        public ChannelBriefService(ITelegramBotClient bot, IUserService userService, DataContext context)
        {
            _bot = bot;
            _userService = userService;
            _context = context;
        }

        public async Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveChannelAbouAsync(int channelId, int userId, string about)
        {
            if (userId == null)
            { 
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("User ID cannot be null.");
            }

            var user = await _userService.GetUserByTelegramIdAsync(userId);

            if (string.IsNullOrWhiteSpace(about))
            { 
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("About section cannot be empty.");
            }

            var channel = await _context.Channels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == channelId && c.UserId == user.Id);

            if (channel == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Channel not found.");
            }

            about = about.Trim();
            channel.About = about;

            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();
            return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Ok(channel);
        }
    }
}
