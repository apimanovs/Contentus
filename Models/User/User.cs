namespace TelegramStatsBot.Models.User
{
    public class User
    {
        public int Id { get; set; }

        public long TelegramId { get; set; }

        public long ChatId { get; set; }

        public string Username { get; set; } = string.Empty;

        public string Plan { get; set; } = "free";

        public DateTime? PremiumUntil { get; set; }

        public bool IsPremium { get; set; } = false;

        public string Language { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public bool IsLanguageConfirmed { get; set; } = false;

        public int? LastMenuMessageId { get; set; }

        public bool HasSeenGuide { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
