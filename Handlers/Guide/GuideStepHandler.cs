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
                        ? "📌 <b>Шаг 1</b>\nДобавь Teleboard в администраторы своего канала..."
                        : "📌 <b>Step 1</b>\nAdd Teleboard to your channel admins...";
                    keyboard = _menuBuilder.GetStepKeyboard(1, user.Language);
                    break;

                case 2:
                    text = user.Language == "ru"
                        ? "📊 <b>Шаг 2</b>\nЯ собираю важные метрики твоего канала..."
                        : "📊 <b>Step 2</b>\nI track your channel's key metrics...";
                    keyboard = _menuBuilder.GetStepKeyboard(2, user.Language);
                    break;

                case 3:
                    text = user.Language == "ru"
                        ? "📅 <b>Шаг 3</b>\nТы можешь планировать публикации..."
                        : "📅 <b>Step 3</b>\nYou can schedule posts...";
                    keyboard = _menuBuilder.GetStepKeyboard(3, user.Language);
                    break;

                case 4:
                    user.HasSeenGuide = true;
                    user.CurrentStep = OnboardingStep.AwaitingChannelLink;

                    await _userService.UpdateUserAsync(user);

                    text = user.Language == "ru"
                        ? "✅ <b>Готово!</b>\nТеперь ты можешь использовать все функции Teleboard!"
                        : "✅ <b>All done!</b>\nYou can now use all features of Teleboard!";

                    await _bot.EditMessageTextAsync(
                        chatId: chatId,
                        messageId: query.Message.MessageId,
                        text: text,
                        parseMode: ParseMode.Html,
                        replyMarkup: null
                    );

                    await _menuService.SetLastMenuMessageId(telegramId, query.Message.MessageId);

                    var askChannelText = user.Language == "ru"
                        ? "📥 Перешли любое сообщение из своего канала, где я уже добавлен в администраторы. Так я смогу начать сбор статистики. Это важно. Не ленись."
                        : "📥 Please forward *any* message from your channel where I’m already an admin. This is how I can start tracking stats. Don’t make it weird.";


                    await _bot.SendTextMessageAsync(chatId, askChannelText);

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
