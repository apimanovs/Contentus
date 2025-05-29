using TelegramStatsBot.Database;
using TelegramStatsBot.Interfaces.User;
using Microsoft.EntityFrameworkCore;

namespace TelegramStatsBot.Services.User
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;

        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<Models.User.User> RegisterUserAsync(long telegramId, long chatId, string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (user == null)
            {
                user = new Models.User.User
                {
                    TelegramId = telegramId,
                    ChatId = chatId,
                    Username = username,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
            }
            else
            {
                user.ChatId = chatId;
                user.Username = username;
                user.IsActive = true;
            }

            await _context.SaveChangesAsync();
            return user;
        }


        public async Task SetUserLanguage(long telegramId, string language, bool confirmed = false)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
            if (user != null)
            {
                user.Language = language;
                user.IsLanguageConfirmed = confirmed;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<Models.User.User> GetUserByTelegramIdAsync(long telegramId)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);
        }

        public async Task UpdateUserAsync(Models.User.User user)
        { 
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> HasAnyChannels(int userId)
        {
            return await _context.Channels.AnyAsync(c => c.UserId == userId);
        }
    }
}
