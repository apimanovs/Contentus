using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu.Guide;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Enums.Onboarding;
using Microsoft.IdentityModel.Tokens;
using TelegramStatsBot.Texsts.Menu;

namespace TelegramStatsBot.Handlers.Guide
{
    public class GuideStepHandler : ICallbackHandler
    {
        public string Key => "guide_step:";

        private readonly ITelegramBotClient _bot;
        private readonly IUserService _userService;
        private readonly IGuideMenuBuilder _menuBuilder;
        private readonly IMainMenuBuilder _mainMenuBuilder;
        private readonly IMenuService _menuService;

        public GuideStepHandler(ITelegramBotClient bot, IUserService userService, IGuideMenuBuilder menuBuilder, IMainMenuBuilder mainMenuBuilder, IMenuService menuService)
        {
            _bot = bot;
            _userService = userService;
            _menuBuilder = menuBuilder;
            _mainMenuBuilder = mainMenuBuilder;
            _menuService = menuService;
        }

        public async Task HandleAsync(CallbackQuery query)
        {
            var chatId = query.Message.Chat.Id;
            var telegramId = query.From.Id;
            var user = await _userService.GetUserByTelegramIdAsync(telegramId);

            var data = query.Data;
            var step = int.Parse(data.Replace("guide_step:", ""));


            string text;
            InlineKeyboardMarkup keyboard;

            switch (step)
            {
                case 1:
                    text = user.Language == "ru"
                        ? "👋 <b>Добро пожаловать в Contentus!</b>\n\nЯ помогу тебе создавать посты, которые будут цеплять твою аудиторию. Всё просто: ты рассказываешь про канал — я генерирую идеи и тексты.\n\n<b>Начнём?</b>"
                        : "👋 <b>Welcome to Contentus!</b>\n\nI help you create posts that grab attention and match your channel’s vibe. You tell me about your channel — I generate ideas and content.\n\n<b>Let’s begin?</b>";
                    keyboard = _menuBuilder.GetStepKeyboard(1, user.Language);
                    break;

                case 2:
                    text = user.Language == "ru"
                        ? "🧠 <b>Шаг 1</b>\nContentus пишет под твой стиль. Для этого мне нужно понять, о чём твой канал и кому ты пишешь.\n\n💬 Расскажи: тематика, ниша, пример поста — всё, что покажет твою подачу."
                        : "🧠 <b>Step 1</b>\nContentus writes in your style. To do that, I need to understand what your channel is about and who you’re writing for.\n\n💬 Tell me: topic, niche, example posts — anything that shows your vibe.";
                    keyboard = _menuBuilder.GetStepKeyboard(2, user.Language);
                    break;

                case 3:
                    text = user.Language == "ru"
                        ? "🎯 <b>Шаг 2</b>\nКакая цель у твоего контента?\n\nПродажи? Вовлечение? Личный бренд? Я подстроюсь под задачу, если ты скажешь, чего хочешь."
                        : "🎯 <b>Step 2</b>\nWhat’s the goal of your content?\n\nSales? Engagement? Personal brand? I’ll tailor the tone and structure if you tell me what you’re aiming for.";
                    keyboard = _menuBuilder.GetStepKeyboard(3, user.Language);
                    break;

                case 4:
                    text = user.Language == "ru"
                        ? "🧩 <b>Шаг 3</b>\nТеперь немного о подаче.\n\nХочешь звучать серьёзно, легко, с юмором или дерзко? Я подстрою тексты под нужный стиль."
                        : "🧩 <b>Step 3</b>\nNow, let’s talk tone.\n\nDo you want to sound serious, casual, witty, or bold? I’ll match the style when writing posts.";
                    keyboard = _menuBuilder.GetStepKeyboard(4, user.Language);
                    break;

                case 5:
                    user.HasSeenGuide = true;
                    user.CurrentStep = OnboardingStep.AddingChannel;
                    await _userService.UpdateUserAsync(user);

                    text = user.Language == "ru"
                                    ? "✅ <b>Обучение пройдено!</b>\nТеперь давай расскажи мне немного о твоём канале, чтобы я мог создавать контент именно под него."
                                    : "✅ <b>You're all set!</b>\nNow tell me a bit about your channel so I can start generating content tailored to it.";

                    await _bot.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: query.Message.MessageId,
                        text: text,
                        parseMode: ParseMode.Html,
                        replyMarkup: null
                    );

                    await _menuService.SetLastMenuMessageId(telegramId, query.Message.MessageId);


                    text = user.Language == "ru"
                                    ? "🚀 Готово начать? \n\nДобавь свой первый канал, чтобы я мог подстроиться под твою аудиторию и начать помогать с контентом. Без этого — ни идей, ни постов. Только тишина и упрёки совести."
                                    : "🚀 Ready to roll?\n\nAdd your first channel so I can tune into your audience and start helping with content. Without it — no posts, no ideas. Just silence and existential guilt.";

                    var hasChannels = await _userService.HasAnyChannels(user.Id);
                    var menu = _mainMenuBuilder.GetMainMenu(user.Language, hasChannels);

                    await _bot.SendTextMessageAsync(
                        chatId: chatId,
                        text: text,
                        replyMarkup: menu
                    );
                    
                    return;

                default:
                    text = "❌ Unknown step.";
                    keyboard = null!;
                    break;
            }

            await _bot.EditMessageTextAsync(
                chatId: chatId,
                messageId: query.Message.MessageId,
                text: text,
                parseMode: ParseMode.Html,
                replyMarkup: keyboard);
        }
    }

}
