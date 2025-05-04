using TelegramStatsBot.Database;
using TelegramStatsBot.Interfaces.Menu;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace TelegramStatsBot.Services.Menu
{
    public class MenuService : IMenuService
    {
        private readonly DataContext _context;

        public MenuService(DataContext context)
        {
            _context = context;
        }

        public async Task SetLastMenuMessageId(long telegramId, int messageId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (user != null)
            {
                user.LastMenuMessageId = messageId;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int?> GetLastMenuMessageId(long telegramId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            return user?.LastMenuMessageId;

        }

        public async Task ClearLastMenuMessageId(long telegramId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.TelegramId == telegramId);

            if (user != null)
            {
                user.LastMenuMessageId = null;
                await _context.SaveChangesAsync();
            }
        }
    }
}
