using Microsoft.EntityFrameworkCore;

namespace TelegramStatsBot.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Models.User.User> Users { get; set; } = null!;
    }
}
