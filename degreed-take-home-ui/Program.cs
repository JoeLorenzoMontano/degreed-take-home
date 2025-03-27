using degreed_core.Clients;
using degreed_core.Services;
using degreed_take_home_ui.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(CanHazApiClient.BASE_ADDRESS) });
builder.Services.AddScoped<ICanHazApiClient, CanHazApiClient>();
builder.Services.AddScoped<IJokeService, JokeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
  app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
