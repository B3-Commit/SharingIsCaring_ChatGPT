using FluentValidation;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace SharingIsCaring.Console.Extensions;

internal static class SettingsValidationExtensions
{
    public static OptionsBuilder<TOptions> ValidateWithFluent<TOptions>(
        this OptionsBuilder<TOptions> optionsBuilder)
        where TOptions : class
    {
        optionsBuilder.Services.AddSingleton<IValidateOptions<TOptions>>(
            provider => new FluentValidationOptions<TOptions>(
                settingsName: optionsBuilder.Name,
                serviceProvider: provider));

        return optionsBuilder;
    }
}

internal class FluentValidationOptions<TOptions>(
    string? settingsName,
    IServiceProvider serviceProvider)
        : IValidateOptions<TOptions>
    where TOptions : class
{
    public ValidateOptionsResult Validate(string? name, TOptions options)
    {
        if (string.IsNullOrWhiteSpace(settingsName) is false && settingsName.Equals(name) is false)
            return ValidateOptionsResult.Skip;

        ArgumentNullException.ThrowIfNull(options);

        using var scope = serviceProvider.CreateAsyncScope();
        var validator = scope.ServiceProvider
            .GetRequiredService<IValidator<TOptions>>();

        var results = validator.Validate(options);

        if (results.IsValid)
            return ValidateOptionsResult.Success;

        var typeName = options.GetType().Name;
        var errors = new List<string>();

        foreach (var errorResult in results.Errors)
        {
            errors.Add($"Validation error: '{typeName}.{errorResult.PropertyName}' "
                + $"with error: [{errorResult.ErrorCode}] '{errorResult.ErrorMessage}'");
        }

        return ValidateOptionsResult.Fail(errors);
    }
}