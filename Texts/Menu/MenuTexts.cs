namespace TelegramStatsBot.Texsts.Menu
{
    public static class MenuTexts
    {
        public static string GetMainMenuTitle(string language, bool hasChannels)
        {
            if (!hasChannels)
            {
                if (language == "ru")
                {
                    return "📭 У тебя ещё не добавлен ни один канал.\n\nЧтобы начать работу, добавь канал с помощью кнопки ниже!";
                }
                else
                {
                    return "📭 You haven't added any channels yet.\n\nTo get started, add a channel using the button below!";
                }
            }
            else
            {
                if (language == "ru")
                {
                    return "Главное меню осноевное";
                }
                else
                {
                    return "Main menu осноевное";
                }
            }
        }

        public static string GetGuideStartText(string language) =>
            language == "ru" ? "🧭 Хочешь пройти краткое обучение, чтобы понять как пользоваться ботом?" :
                               "🧭 Want to go through a short guide on how to use Teleboard?";
    }
}
