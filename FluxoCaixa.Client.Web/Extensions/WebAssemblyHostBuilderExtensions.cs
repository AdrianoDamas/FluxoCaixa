using Blazored.LocalStorage;
using FluxoCaixa.Client.Web.Infrastructure.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using MudBlazor.Services;
using System;
using System.Net.Http;
using FluxoCaixa.Models.Security;
using FluxoCaixa.Client.Web.Services.Preferences;
using Toolbelt.Blazor.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using FluxoCaixa.Client.Web.Services.Security;
using FluxoCaixa.Client.Web.Services.Interceptors;
using FluxoCaixa.Client.Services.Security;
using FluxoCaixa.Client.Services.Manutencao;
using FluxoCaixa.Client.Services.Operacional;
using FluxoCaixa.Client.Services.Apoio;
using NuvTools.Security.Extensions;
using BlazorDownloadFile;

namespace FluxoCaixa.Client.Web.Extensions;

public static class WebAssemblyHostBuilderExtensions
{
    private const string ClientName = "FluxoCaixa.API";

    public static WebAssemblyHostBuilder AddApplication(this WebAssemblyHostBuilder builder)
    {
        builder.AddRootComponents().AddClientServices().AddFeatures();
        return builder;
    }

    private static WebAssemblyHostBuilder AddRootComponents(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");

        return builder;
    }

    private static WebAssemblyHostBuilder AddClientServices(this WebAssemblyHostBuilder builder)
    {
        builder
            .Services
            .AddAuthorizationCore(options =>
            {
                foreach (var item in Permissions.GetAllPermissions())
                {
                    options.AddPolicyWithRequiredPermissionClaim(item.Value, item.Value);
                }
            })
            .AddBlazorDownloadFile()
            .AddBlazoredLocalStorage()
            .AddLocalization()
            .AddMudServices(
            configuration =>
            {
                configuration.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
                configuration.SnackbarConfiguration.HideTransitionDuration = 100;
                configuration.SnackbarConfiguration.ShowTransitionDuration = 100;
                configuration.SnackbarConfiguration.VisibleStateDuration = 3000;
                configuration.SnackbarConfiguration.ShowCloseIcon = false;
            })
            .AddScoped<PreferenceService>()
            .AddScoped<ApplicationStateProvider>()
            .AddScoped<AuthenticationStateProvider, ApplicationStateProvider>()
            .AddTransient<AuthenticationHeaderHandler>()
            .AddScoped(sp => sp
                .GetRequiredService<IHttpClientFactory>()
                .CreateClient(ClientName).EnableIntercept(sp))
            .AddHttpClient(ClientName, client => client.BaseAddress = new Uri(builder.Configuration.GetValue<string>("APIConfiguracao:Principal:Endereco")))
            .AddHttpMessageHandler<AuthenticationHeaderHandler>();
        builder.Services.AddHttpClientInterceptor();
        return builder;
    }

    private static WebAssemblyHostBuilder AddFeatures(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddTransient<HttpInterceptorService>();
        builder.Services.AddTransient<UserService>();
        builder.Services.AddTransient<RoleService>();
        builder.Services.AddTransient<AccountService>();
        builder.Services.AddTransient<AuthenticationService>();

        builder.Services.AddTransient<TipoTransacaoService>();
        builder.Services.AddTransient<CategoriaService>();
        builder.Services.AddTransient<ContaService>();

        builder.Services.AddTransient<TransacaoService>();
        
        return builder;
    }
}