using OpenAI.Chat;
using TelegramContentusBot.Models.OpenAI;
using TelegramStatsBot.Database;
using Microsoft.EntityFrameworkCore;
using TelegramStatsBot.Models.User;
using TelegramStatsBot.Models.Channel;

namespace TelegramContentusBot.Requests.Posts
{
    public class PostGenerationRequest

    {
        private readonly OpenAiOptions _options;
        private readonly DataContext _context;

        public PostGenerationRequest(OpenAiOptions options, DataContext context)
        {
            _options = options;
            _context = context;
        }

        public async Task<string> GeneratePostAsync(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return "Пользователь не найден.";

            var channel = await _context.Channels.FirstOrDefaultAsync(c => c.UserId == userId);
            if (channel == null) return "Канал не найден.";

            var prompt = $"""
                        Ты — опытный копирайтер, создающий интересные и вовлекающие посты для Telegram-каналов.

                        📌 Создай уникальный пост, используя следующие данные о канале:

                        1. <b>Описание канала (About)</b>: {channel.About}  
                        2. <b>Цель канала (Content Goal)</b>: {channel.ContentGoal}  
                        3. <b>Предпочитаемый стиль (Style Preference)</b>: {channel.StylePreference}  
                        4. <b>Целевая аудитория (Target Audience)</b>: {channel.TargetAudience}  
                        5. <b>Пример поста (Example Post)</b>: {channel.ExamplePosts}

                        📄 <b>Требования к посту:</b>  
                        - Используй <b>HTML-разметку</b> (например, <b>жирный</b>, <i>курсив</i>, <u>подчёркнутый</u>, <code>моноширинный</code>) для выделения ключевых фраз.  
                        - Начни с <b>вовлекающей фразы или заголовка</b>, чтобы сразу захватить внимание.  
                        - Раскрой основную идею в теле поста, соблюдая стиль канала.  
                        - Заверши <i>призывом к действию</i> — задавай вопрос, призови поделиться, написать или задуматься.  
                        - Общая длина поста — <b>500–1000 символов</b>.  
                        - Избегай повторов и клише. Пиши <u>живым языком</u>.

                        🎯 Не пиши пояснений — <b>выведи только готовый HTML-текст поста</b>.
                        """;

            var client = new ChatClient(model: "gpt-4o", apiKey: _options.ApiKey);
            ChatCompletion completion = await client.CompleteChatAsync(prompt);

            Console.WriteLine($"Generated post for user {userId}: {completion.Content[0].Text}");
            Console.WriteLine($"Generated post for prompt {prompt}");

            return completion.Content[0].Text;
        }
    }
}
