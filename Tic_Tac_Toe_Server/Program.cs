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
    app.AddJsonFile("appsettings.json", true, true)
        .AddEnvironmentVariables();
})
.ConfigureLogging(loggerBuilder =>
{
    loggerBuilder.ClearProviders();
    loggerBuilder.AddSerilog(new LoggerConfiguration()
       .WriteTo.File("app.log")
       .CreateLogger());
})
.ConfigureServices(service =>
{
    service.AddOptions();
    service.AddControllers();
    service.AddSingleton<IAccountService, AccountService>();
    service.AddSingleton<IRoomService, RoomService>();
    service.AddSingleton<IBlocker, UserBlocker>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});


await app.RunAsync();
