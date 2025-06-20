using TelegramContentusBot.Enums.ChannelDetails;
using TelegramStatsBot.Models.User;

namespace TelegramStatsBot.Models.Channel
{
    public class Channel
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public User.User User { get; set; }

        public string? ChannelUsername { get; set; }
        public string? ChannelTitle { get; set; }
        public string? ChannelLink { get; set; }
        public long ChannelId { get; set; }
        public bool IsBotAdmin { get; set; }

        public string? About { get; set; }

        public string? TargetAudience { get; set; }

        public string? ContentGoal { get; set; }

        public string? StylePreference { get; set; }

        public string? ExamplePosts { get; set; }

        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        public ChannelDetailsSteps ChannelDetailsStep { get; set; } = ChannelDetailsSteps.None;
    }
}
