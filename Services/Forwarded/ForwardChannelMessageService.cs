using Microsoft.EntityFrameworkCore;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramContentusBot.Interfaces.Forwarded.Channel;
using TelegramStatsBot.Database;
using TelegramStatsBot.Models.Result;
using TelegramStatsBot.Interfaces.User;

namespace TelegramStatsBot.Services.Forwarded
{
    public class ForwardChannelMessageService : IForwardChannelMessageService
    {
        private readonly ITelegramBotClient _bot;
        private readonly DataContext _context;
        private readonly IUserService _userService;

        public ForwardChannelMessageService(ITelegramBotClient bot, DataContext dataContext, IUserService userService)
        {
            _bot = bot;
            _context = dataContext;
            _userService = userService;
        }

        public async Task<OperationResult<Models.Channel.Channel>> ProcessForwardedChannelAsync(Telegram.Bot.Types.Message message, int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if (message.ForwardFromChat == null || message.ForwardFromChat.Type != Telegram.Bot.Types.Enums.ChatType.Channel)
            {
                return OperationResult<Models.Channel.Channel>.Fail("⚠️ Это сообщение не переслано из канала");
            }

            var chat = message.ForwardFromChat;

            var exists = await _context.Channels.AsNoTracking()
                .FirstOrDefaultAsync(c => c.ChannelId == chat.Id && c.UserId == userId);

            if (exists != null)
            {
                return OperationResult<Models.Channel.Channel>.Fail("📌 Ты уже добавил этот канал.");
            }

            var botInfo = await _bot.GetMeAsync();

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

            user.CurrentStep = Enums.Onboarding.OnboardingStep.None;
            user.ChannelDetailsStep = TelegramContentusBot.Enums.ChannelDetails.ChannelDetailsSteps.About;
            user.LastEditedChannelId = channel.Id;

            await _context.SaveChangesAsync();
            _context.Users.Update(user);

            return OperationResult<Models.Channel.Channel>.Ok(channel);
        }
    }
}