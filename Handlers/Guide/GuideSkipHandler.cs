using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu;
using Telegram.Bot.Types.Enums;

public class GuideSkipHandler : ICallbackHandler
{
    public string Key => "guide_skip";

    private readonly ITelegramBotClient _bot;
    private readonly IUserService _userService;
    private readonly IMainMenuBuilder _menuBuilder;
    private readonly IMenuService _menuService;

    public GuideSkipHandler(ITelegramBotClient bot, IUserService userService, IMainMenuBuilder menuBuilder, IMenuService menuService)
    {
        _bot = bot;
        _userService = userService;
        _menuBuilder = menuBuilder;
        _menuService = menuService;
    }

    public async Task HandleAsync(CallbackQuery query)
    {
        var telegramId = query.From.Id;
        var chatId = query.Message.Chat.Id;

        var user = await _userService.GetUserByTelegramIdAsync(telegramId);
        user.HasSeenGuide = true;
        await _userService.UpdateUserAsync(user);

        var menu = _menuBuilder.GetMainMenu(user.Language);
        var menuText = user.Language == "ru" ? "📋 Главное меню:" : "📋 Main menu:";

        await _bot.EditMessageTextAsync(
            chatId: chatId,
            messageId: query.Message.MessageId,
            text: menuText,
            parseMode: ParseMode.Html,
            replyMarkup: menu
        );

        await _menuService.SetLastMenuMessageId(telegramId, query.Message.MessageId);
    }
}
