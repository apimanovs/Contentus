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
using Microsoft.VisualBasic;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrWhiteSpace(about))
            { 
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("About section cannot be empty.");
            }

            if (about.Length >= 1000)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Текст не длиннее 1000 символов.");
            }

            var channel = await _context.Channels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == channelId && c.UserId == user.Id);

            if (channel == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Channel not found.");
            }

            about = about.Trim().ToLower();
            channel.About = about;

            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();

            user.ChannelDetailsStep = Enums.ChannelDetails.ChannelDetailsSteps.TargetAudience;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Ok(channel);
        }

        public async Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveTargetAudienceAsync(int channelId, int userId, string targetAudience)
        {
            if (userId == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("User ID cannot be null.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrWhiteSpace(targetAudience))
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("About section cannot be empty.");
            }

            if (targetAudience.Length >= 1000)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Текст не длиннее 1000 символов.");
            }

            var channel = await _context.Channels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == channelId && c.UserId == user.Id);

            if (channel == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Channel not found.");
            }

            targetAudience = targetAudience.Trim().ToLower();
            channel.TargetAudience = targetAudience;

            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();

            user.ChannelDetailsStep = Enums.ChannelDetails.ChannelDetailsSteps.StylePreference;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Ok(channel);
        }


        public async Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SavePreferedStyleAsync(int userId, int channelId, string preferedStyle)
        {
            if (userId == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("User ID cannot be null.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrWhiteSpace(preferedStyle))
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("About section cannot be empty.");
            }

            if (preferedStyle.Length >= 1000)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Текст не длиннее 1000 символов.");
            }

            var channel = await _context.Channels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == channelId && c.UserId == user.Id);

            if (channel == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Channel not found.");
            }

            preferedStyle = preferedStyle.Trim().ToLower();
            channel.StylePreference = preferedStyle;

            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();

            user.ChannelDetailsStep = Enums.ChannelDetails.ChannelDetailsSteps.ContentGoal;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Ok(channel);
        }

        public async Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveContentGoalAsync(int userId, int channelId, string contentGoal)
        {
            if (userId == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("User ID cannot be null.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrWhiteSpace(contentGoal))
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("About section cannot be empty.");
            }

            if (contentGoal.Length >= 1000)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Текст не длиннее 1000 символов.");
            }

            var channel = await _context.Channels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == channelId && c.UserId == user.Id);

            if (channel == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Channel not found.");
            }

            contentGoal = contentGoal.Trim().ToLower();
            channel.ContentGoal = contentGoal;

            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();

            user.ChannelDetailsStep = Enums.ChannelDetails.ChannelDetailsSteps.ExamplePost;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Ok(channel);
        }


        public async Task<OperationResult<TelegramStatsBot.Models.Channel.Channel>> SaveExamplePostAsync(int userId, int channelId, string examplePost)
        {
            if (userId == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("User ID cannot be null.");
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (string.IsNullOrWhiteSpace(examplePost))
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("About section cannot be empty.");
            }

            if (examplePost.Length >= 1000)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Текст не длиннее 1000 символов.");
            }

            var channel = await _context.Channels.AsNoTracking().FirstOrDefaultAsync(c => c.Id == channelId && c.UserId == user.Id);

            if (channel == null)
            {
                return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Fail("Channel not found.");
            }

            examplePost = examplePost.Trim().ToLower();
            channel.ExamplePosts = examplePost;

            _context.Channels.Update(channel);
            await _context.SaveChangesAsync();

            user.ChannelDetailsStep = Enums.ChannelDetails.ChannelDetailsSteps.None;

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return OperationResult<TelegramStatsBot.Models.Channel.Channel>.Ok(channel);
        }
    }
}