using TelegramStatsBot.Models.User;

namespace TelegramStatsBot.Models.Channel
{
    public class Channel
    {
        public int Id { get; set; }

        public long TelegramUserId { get; set; }
        public User.User User { get; set; }

        public string ChannelUsername { get; set; }
        public string ChannelTitle { get; set; }
        public long ChannelId { get; set; }

        public bool IsBotAdmin { get; set; }

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
    }
}
