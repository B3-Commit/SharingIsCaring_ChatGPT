using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using SharingIsCaring.Core.Models.OpenAI;

namespace SharingIsCaring.Core.Services;

public interface IOpenAIClientService
{
}

public class OpenAIClientService : IOpenAIClientService
{
    private readonly OpenAIClient _openAIClient;
    private readonly ILogger<OpenAIClientService> _logger;
    private readonly OpenAIClientServiceOptions _serviceOptions;

    public OpenAIClientService(
        ILogger<OpenAIClientService> logger,
        IOptions<OpenAIClientServiceOptions> options)
    {
        _logger = logger;
        _serviceOptions = options.Value;

        _openAIClient = new OpenAIClient(
            endpoint: new Uri(_serviceOptions.ServiceUri),
            keyCredential: new AzureKeyCredential(_serviceOptions.ServiceKey));
    }

    public OpenAIClientService(
        ILogger<OpenAIClientService> logger,
        Action<OpenAIClientServiceOptions> options)
    {
        _logger = logger;
        _serviceOptions = new();
        options(_serviceOptions);

        _openAIClient = new OpenAIClient(
            endpoint: new Uri(_serviceOptions.ServiceUri),
            keyCredential: new AzureKeyCredential(_serviceOptions.ServiceKey));
    }

    public async Task<string> GetChatCompletionsAsync(
        ChatCompletionInput input,
        ChatCompletionsOptions options = null!,
        CancellationToken cancellationToken = default)
    {
        _logger.BeginScope($"[{nameof(OpenAIClientService)}]");

        if (options is null)
            options = GetDefaultOptions();

        // Add assistant message if missing, otherwise skip it
        if (options.Messages.Any(e => e.Role != ChatRole.Assistant))
            options.Messages.Add(new ChatMessage(ChatRole.Assistant, input.AssistantMessage));

        // Add messages in order
        foreach (var message in input.Messages.OrderByDescending(e => e.Created))
        {
            options.Messages.Add(new ChatMessage(message.role, message.message));
        }

        _logger.LogTrace(message: "Sending query to OpenAI service");

        Response<ChatCompletions> response = await _openAIClient
            .GetChatCompletionsAsync(
                chatCompletionsOptions: options,
                cancellationToken: cancellationToken);

        ChatCompletions completions = response.Value;

        return completions.Choices.Any()
            ? completions.Choices[0].Message.Content
            : string.Empty;
    }

    private ChatCompletionsOptions GetDefaultOptions(
        string deployment = "")
        => new ChatCompletionsOptions
        {
            Temperature = (float)0.7,
            NucleusSamplingFactor = (float)0.95,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
            DeploymentName = string.IsNullOrWhiteSpace(deployment)
                ? _serviceOptions.Deployments.FirstOrDefault()?.Name
                : _serviceOptions.Deployments.FirstOrDefault(e => e.Equals(deployment))?.Name
        };
}