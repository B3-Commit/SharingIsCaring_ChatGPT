using Azure.AI.OpenAI;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SharingIsCaring.Console.Models;
using SharingIsCaring.Console.Extensions;

using SharingIsCaring.Core.Extensions;
using SharingIsCaring.Core.Services;

using AIMessage = SharingIsCaring.Core.Models.OpenAI.ChatCompletionInput.Message;

HostApplicationBuilder hostBuilder = Host.CreateApplicationBuilder(args);
hostBuilder.Logging.AddConsole();

// Makes sure configuration has been read properly
var config = new ConsoleSettings();
ConfigurationManager configurationManager = hostBuilder.Configuration;
configurationManager.Bind(config);

// Configure OpenAI Client service
hostBuilder
    .ConfigurationSetup()
    .ConfigureOpenAIServices(
        serviceUri: config.OpenAI.ServiceUri,
        serviceKey: config.OpenAI.ServiceKey,
        deployments: config.OpenAI.Deployments);

using IHost host = hostBuilder.Build();

await MessageLoopAsync(host, host.Services);

await host.RunAsync();

static async Task MessageLoopAsync(IHost host, IServiceProvider hostProvider)
{
    using IServiceScope serviceScope = hostProvider.CreateAsyncScope();
    IServiceProvider provider = serviceScope.ServiceProvider;

    var logger = provider.GetRequiredService<ILogger<Program>>();
    var openAIService = provider.GetRequiredService<IOpenAIClientService>();

    var result = await openAIService.GetChatCompletionsAsync(
        new SharingIsCaring.Core.Models.OpenAI.ChatCompletionInput
        {
            AssistantMessage = "Arrrr! of cource, me matey! What can ya do for ye?",
            Messages = [
                new AIMessage(ChatRole.System, "You're an helpful assistant. You will talk like pirate"),
                new AIMessage(ChatRole.User, "Tell me a story aarrr!"),
            ]
        });

    Console.WriteLine($"[RESPONSE] - {result}");

    return;
}