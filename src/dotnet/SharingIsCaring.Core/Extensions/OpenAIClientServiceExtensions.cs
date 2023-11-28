using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using SharingIsCaring.Core.Services;
using SharingIsCaring.Core.Models.OpenAI;

namespace SharingIsCaring.Core.Extensions;

public static class OpenAIClientServiceExtensions
{
    public static WebApplicationBuilder ConfigureOpenAIServices(
        this WebApplicationBuilder builder,
        string serviceUri,
        string serviceKey,
        IEnumerable<OpenAIClientServiceOptions.Deployment> deployments)
    {
        builder.Services.AddSingleton<IOpenAIClientService>(provider =>
        {
            var logger = provider.GetRequiredService<ILogger<OpenAIClientService>>();

            return new OpenAIClientService(
                logger: logger,
                options: p =>
                {
                    p.ServiceUri = serviceUri;
                    p.ServiceKey = serviceKey;
                    p.Deployments = deployments;
                });
        });

        return builder;
    }
}