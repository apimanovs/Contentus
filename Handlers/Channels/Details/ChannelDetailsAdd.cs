using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Message;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Menu.Guide;
using TelegramStatsBot.Enums.Onboarding;
using Telegram.Bot.Requests;
using TelegramStatsBot.Texsts.Menu;


namespace TelegramContentusBot.Handlers.Channels.Details
{
    public class ChannelDetailsAdd : IMessageHandler
    {
        private readonly IUserService _userService;
        private readonly ITelegramBotClient _bot;

        public ChannelDetailsAdd(IUserService userService, ITelegramBotClient bot)
        {
            _userService = userService;
            _bot = bot;
        }

        public string Command => "";
        public async Task HandleAsync(Message message)
        {
            var user = await _userService.GetUserByTelegramIdAsync(message.From.Id);

            if (user == null)
            {
                await _bot.SendTextMessageAsync(
                    chatId: message.Chat.Id,
                    text: "❌ User not found. Please start the bot with /start command.",
                    parseMode: ParseMode.Html
                );
                return;
            }

            if (user.ChannelDetailsStep == Enums.ChannelDetails.ChannelDetailsSteps.None 
                || user.ChannelDetailsStep == Enums.ChannelDetails.ChannelDetailsSteps.Done)
            {
                return;  
            }

            // getting channel data
        }
    }
    
}
