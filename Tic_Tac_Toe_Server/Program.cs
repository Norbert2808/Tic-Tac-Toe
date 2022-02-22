using Tic_Tac_Toe.Server.CustomLogger;
using Tic_Tac_Toe.Server.Service;
using Tic_Tac_Toe.Server.Tools;

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
    var filepath = Path.Combine(Directory.GetCurrentDirectory(), "app.log");
    loggerBuilder.AddFile(filepath);
})
.ConfigureServices(services =>
{
    services.AddOptions();
    services.AddControllers();
    services.AddSingleton<IAccountService, AccountService>();
    services.AddSingleton<IBlocker, UserBlocker>();
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
