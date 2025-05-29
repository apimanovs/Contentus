using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu;
using Telegram.Bot.Types.Enums;
using TelegramStatsBot.Enums.Onboarding;
using static System.Net.Mime.MediaTypeNames;

public class GuideSkipHandler : ICallbackHandler
{
    public string Key => "guide_skip";

    private readonly ITelegramBotClient _bot;
    private readonly IUserService _userService;
    private readonly IMainMenuBuilder _menuBuilder;
    private readonly IMenuService _menuService;
    private readonly IMainMenuBuilder _mainMenuBuilder;

    public GuideSkipHandler(ITelegramBotClient bot, IUserService userService, IMainMenuBuilder menuBuilder, IMenuService menuService, IMainMenuBuilder mainMenuBuilder)
    {
        _bot = bot;
        _userService = userService;
        _menuBuilder = menuBuilder;
        _menuService = menuService;
        _mainMenuBuilder = mainMenuBuilder;
    }

    public async Task HandleAsync(CallbackQuery query)
    {
        var telegramId = query.From.Id;
        var chatId = query.Message.Chat.Id;

        var user = await _userService.GetUserByTelegramIdAsync(telegramId);
        user.HasSeenGuide = true;
        await _userService.UpdateUserAsync(user);

        user.CurrentStep = OnboardingStep.AddingChannel;
        await _userService.UpdateUserAsync(user);

        var confirmationText = user.Language == "ru"
                    ? "✅ <b>Обучение пропущено!</b>\nТеперь давай расскажи немного о своём канале, чтобы я мог начать помогать с контентом."
                    : "✅ <b>Guide skipped!</b>\nNow tell me a bit about your channel so I can start helping you with content.";

        await _bot.EditMessageTextAsync(
            chatId: chatId,
            messageId: query.Message.MessageId,
            text: confirmationText,
            parseMode: ParseMode.Html,
            replyMarkup: null
        );

        var text = user.Language == "ru"
                                   ? "🚀 Готово начать? \n\nДобавь свой первый канал, чтобы я мог подстроиться под твою аудиторию и начать помогать с контентом. Без этого — ни идей, ни постов. Только тишина и упрёки совести."
                                   : "🚀 Ready to roll?\n\nAdd your first channel so I can tune into your audience and start helping with content. Without it — no posts, no ideas. Just silence and existential guilt.";

        var hasChannels = await _userService.HasAnyChannels(user.Id);
        var menu = _mainMenuBuilder.GetMainMenu(user.Language, hasChannels);

        await _bot.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            replyMarkup: menu
        );

        await _menuService.SetLastMenuMessageId(telegramId, query.Message.MessageId);
    }
}
