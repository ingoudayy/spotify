using SpotifyTopSongsApp;
using SpotifyTopSongsApp.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var spotifyOptions = new SpotifyOptions
{
    ClientId = "22d91b7396664f869cd21f32d5afacd2",
    ClientSecret = "86463fcebde04ce3a3c833ca4fe8327d"
};

builder.Services.AddSingleton(spotifyOptions);
builder.Services.AddScoped<SpotifyService>();

//  Enregistrer dans le conteneur DI
builder.Services.AddSingleton(spotifyOptions);
builder.Services.AddScoped<SpotifyService>();

await builder.Build().RunAsync();
