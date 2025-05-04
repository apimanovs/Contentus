using TelegramStatsBot.Database;
using TelegramStatsBot.Services.Menu;
using Microsoft.EntityFrameworkCore;
using TelegramStatsBot.Interfaces.Menu;
using System.Threading.Tasks;

namespace TelegramStatsBot.Interfaces.Menu
{
    public interface IMenuService
    {
        Task SetLastMenuMessageId(long telegramId, int messageId);
        Task<int?> GetLastMenuMessageId(long telegramId);
        Task ClearLastMenuMessageId(long telegramId);
    }
}
