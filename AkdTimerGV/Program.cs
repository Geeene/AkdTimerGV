using AkdTimerGV.Components;
using Microsoft.AspNetCore.DataProtection;
using Plk.Blazor.DragDrop;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(System.Environment.GetEnvironmentVariable("KeysLocation") == null ? "/home/web/Apps/AkdTimerGV/Keys" : System.Environment.GetEnvironmentVariable("KeysLocation")));
builder.Services.AddBlazorDragDrop();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
