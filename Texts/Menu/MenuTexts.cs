namespace TelegramStatsBot.Texsts.Menu
{
    public static class MenuTexts
    {
        public static string GetMainMenuTitle(string language) =>
            language == "ru" ? "📋 Главное меню:" : "📋 Main menu:";

        public static string GetGuideStartText(string language) =>
            language == "ru" ? "🧭 Хочешь пройти краткое обучение, чтобы понять как пользоваться ботом?" :
                               "🧭 Want to go through a short guide on how to use Teleboard?";
    }
}
