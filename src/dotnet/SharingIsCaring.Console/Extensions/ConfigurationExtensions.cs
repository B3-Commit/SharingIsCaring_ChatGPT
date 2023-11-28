using FluentValidation;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SharingIsCaring.Console.Models;

namespace SharingIsCaring.Console.Extensions;

internal static class ConfigurationExtensions
{
    public static HostApplicationBuilder ConfigurationSetup(
        this HostApplicationBuilder builder)
    {
        builder.Configuration
            .AddJsonFile("appsettings.json",
                optional: true,
                reloadOnChange: true);
            // .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json",
            //     optional: true,
            //     reloadOnChange: true);

        builder.Services.AddScoped<IValidator<ConsoleSettings>, ConsoleSettingsValidator>();
        builder.Services.AddOptions<ConsoleSettings>()
            .BindConfiguration(nameof(ConsoleSettings))
            .ValidateWithFluent()
            .ValidateOnStart();

        return builder;
    }
}

internal class ConsoleSettingsValidator : AbstractValidator<ConsoleSettings>
{
}