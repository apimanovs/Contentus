using Telegram.Bot;
using Microsoft.EntityFrameworkCore;
using TelegramStatsBot.Database;
using TelegramStatsBot.Interfaces.User;
using TelegramStatsBot.Interfaces.Message;
using TelegramStatsBot.Interfaces.Callback;
using TelegramStatsBot.Interfaces.Menu;
using TelegramStatsBot.Interfaces.Menu.Main;
using TelegramStatsBot.Handlers.Commands;
using TelegramStatsBot.Handlers.Language;
using TelegramStatsBot.Services.User;
using TelegramStatsBot.Services.Menu;
using TelegramStatsBot.Builders.Menu;
using TelegramStatsBot.Dispatchers.Callback;
using TelegramStatsBot.Dispatchers.Message;
using TelegramStatsBot.Interfaces.Menu.Guide;
using TelegramStatsBot.Builders.Menu.Guide;
using TelegramStatsBot.Handlers.Guide;
using TelegramStatsBot.Interfaces.Handler;
using TelegramStatsBot.Handlers.Forwarded;
using TelegramStatsBot.Services;
using TelegramContentusBot.Interfaces.Forwarded.Channel;
using TelegramStatsBot.Services.Forwarded;
using TelegramContentusBot.Interfaces.Channel;
using TelegramContentusBot.Services.Channel;
using TelegramContentusBot.Handlers.Channels.Details;
using TelegramContentusBot.Models.OpenAI;
using Microsoft.Extensions.Options;
using TelegramContentusBot.Requests.Posts;

var builder = WebApplication.CreateBuilder(args);

var botToken = builder.Configuration["BOT_TOKEN"];

builder.Services.Configure<OpenAiOptions>(builder.Configuration.GetSection("OpenAI"));
builder.Services.AddSingleton<OpenAiOptions>(sp =>
    sp.GetRequiredService<IOptions<OpenAiOptions>>().Value);


builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddSingleton<ITelegramBotClient>(new TelegramBotClient(botToken));

builder.Services.AddControllers();


builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageHandler, StartCommandHandler>();
builder.Services.AddScoped<ICallbackHandler, LanguageCallbackHandler>();
builder.Services.AddScoped<IMainMenuBuilder, MainMenuBuilder>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IGuideMenuBuilder, GuideMenuBuilder>();
builder.Services.AddScoped<ICallbackHandler, GuideStepHandler>();
builder.Services.AddScoped<ICallbackHandler, GuideSkipHandler>();
builder.Services.AddScoped<IForwardedMessageHandler, ForwardedMessageHandler>();
builder.Services.AddScoped<IForwardChannelMessageService, ForwardChannelMessageService>();
builder.Services.AddScoped<IChannelBriefService, ChannelBriefService>();
builder.Services.AddScoped<IMessageHandler, ChannelBriefHandler>();
builder.Services.AddScoped<PostGenerationRequest>();


builder.Services.AddScoped<MessageDispatcher>();
builder.Services.AddScoped<CallbackDispatcher>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

//var scope = app.Services.CreateScope(); 
//var postGen = scope.ServiceProvider.GetRequiredService<PostGenerationRequest>(); 
//postGen.GeneratePostAsync(15);

app.Run();