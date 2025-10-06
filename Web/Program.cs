using Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BlockChainService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

app.UseStaticFiles();

app.UseRouting();

app.MapControllers();

app.Run();