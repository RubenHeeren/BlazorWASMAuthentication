using Blazored.LocalStorage;
using Client;
using Client.Providers;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

builder.Services.AddSingleton<HttpClient>();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<AppAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider 
    => provider.GetRequiredService<AppAuthenticationStateProvider>());

await builder.Build().RunAsync();
