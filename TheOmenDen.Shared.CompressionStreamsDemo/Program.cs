using BlazorDexie.Extensions;
using Blazored.LocalStorage;
using Blazorise;
using Blazorise.FluentUI2;
using Blazorise.Icons.FluentUI;
using Blazorise.LoadingIndicator;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TheOmenDen.Shared.CompressionStreamsDemo;
using TheOmenDen.Shared.CompressionStreamsWrapper;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazorise(options => options.Immediate = true)
    .AddFluentUI2Providers()
    .AddFluentUIIcons()
    .AddLoadingIndicator();

builder.Services.AddScopedCompressionService();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddDexieWrapper();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
