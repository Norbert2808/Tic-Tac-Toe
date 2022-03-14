using Serilog;
using TicTacToe.Server.Services;
using TicTacToe.Server.Services.Impl;
using TicTacToe.Server.Tools;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

var host = builder.Host;

host.ConfigureAppConfiguration(app =>
{
    _ = app.AddJsonFile("appsettings.json", true, true)
        .AddEnvironmentVariables();
})
.ConfigureLogging(loggerBuilder =>
{
    _ = loggerBuilder.ClearProviders();
    _ = loggerBuilder.AddSerilog(new LoggerConfiguration()
        .WriteTo.File("app.log")
        .CreateLogger());
})
.ConfigureServices(service =>
{
    _ = service.AddOptions()
        .AddSingleton<IAccountService, AccountService>()
        .AddSingleton<IRoomService, RoomService>()
        .AddSingleton<IRoundService, RoundService>()
        .AddSingleton<IStatisticService, StatisticService>()
        .AddSingleton<IBlocker, UserBlocker>()
        .AddControllers();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    _ = app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllers();
});


await app.RunAsync();
