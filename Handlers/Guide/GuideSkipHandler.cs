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
                                ? "📩 Пришли <b>пересланное сообщение</b> из своего канала. Это поможет мне собрать информацию и продолжить настройку."
                                : "📩 Please forward a <b>message from your channel</b>. This will help me gather info and proceed.";

        await _bot.SendTextMessageAsync(
            chatId: chatId,
            text: text,
            parseMode: ParseMode.Html
        );

        await _menuService.SetLastMenuMessageId(telegramId, query.Message.MessageId);
    }
}
