using degreed.Clients;
using degreed.Services;
using degreed.Services.Interfaces;
using degreed_take_home_ui.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Configure Redis cache if connection string is available
var redisConnection = Environment.GetEnvironmentVariable("REDIS_CONNECTION") ?? 
                     builder.Configuration["Redis:ConnectionString"];

if (!string.IsNullOrEmpty(redisConnection))
{
    // Register Redis cache service
    builder.Services.AddSingleton<ICacheService>(sp => new RedisCacheService(redisConnection));
    Console.WriteLine($"Redis cache configured with connection: {redisConnection}");
}
else
{
    Console.WriteLine("Redis cache not configured. Running without caching.");
}

// Register API client and joke service
builder.Services.AddScoped<ICanHazApiClient, CanHazApiClient>();
builder.Services.AddScoped<IJokeService>(sp => {
    var apiClient = sp.GetRequiredService<ICanHazApiClient>();
    var cacheService = sp.GetService<ICacheService>(); // May be null if Redis is not configured
    return new JokeService(apiClient, cacheService);
});

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