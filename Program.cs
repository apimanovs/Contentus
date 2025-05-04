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

var builder = WebApplication.CreateBuilder(args);

var botToken = builder.Configuration["BOT_TOKEN"];

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

app.Run();
