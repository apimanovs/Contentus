namespace TelegramStatsBot.Interfaces.User
{
    public interface IUserService
    {
        /// <summary>
        /// Registers a new user or updates an existing one by their Telegram ID.
        /// If the user already exists, updates their ChatId and Username.
        /// </summary>
        /// <param name="telegramId">The unique Telegram user ID.</param>
        /// <param name="chatId">The chat ID from which the message was sent.</param>
        /// <param name="username">The user's Telegram username.</param>
        /// <returns>The created or updated User object.</returns>
        Task<Models.User.User> RegisterUserAsync(long telegramId, long chatId, string username);


        /// <summary>
        /// Sets the interface language for the user.
        /// </summary>
        /// <param name="telegram">The Telegram ID of the user.</param>
        /// <param name="language">The selected language code (e.g., "en", "ru").</param>
        Task SetUserLanguage(long telegram, string language, bool conformed = false);

        Task<Models.User.User> GetUserByTelegramIdAsync(long telegramId);

        Task UpdateUserAsync(Models.User.User user);

        Task<bool> HasAnyChannels(int userId);
    }
}
