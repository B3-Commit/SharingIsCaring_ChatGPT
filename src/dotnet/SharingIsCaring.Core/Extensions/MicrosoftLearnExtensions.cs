using System.Net.Http.Headers;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

using SharingIsCaring.Core.Services;

namespace SharingIsCaring.Core.Extensions;

public static class MicrosoftLearnExtensions
{
    public static WebApplicationBuilder ConfigureMicrosoftLearnSearchService(
        this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IMicrosoftLearnSearchService, MicrosoftLearnSearchService>();

        return builder;
    }

    public static WebApplicationBuilder ConfigureMicrosoftLearnHttpClients(
        this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient(
            name: nameof(MicrosoftLearnSearchService),
            configureClient: opt =>
            {
                opt.BaseAddress = new Uri("https://learn.microsoft.com");

                // Clear out default junk headers
                opt.DefaultRequestHeaders.Clear();

                opt.DefaultRequestHeaders.Accept.Add(
                    item: new MediaTypeWithQualityHeaderValue("application/json"));
            });

        return builder;
    }
}