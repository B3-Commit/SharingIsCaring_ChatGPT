using Azure.AI.OpenAI;

namespace SharingIsCaring.Core.Models.OpenAI;

public class ChatCompletionInput
{
    public string AssistantMessage { get; set; } = null!;
    public IEnumerable<Message> Messages { get; set; }
        = Enumerable.Empty<Message>();

    public record Message(ChatRole role, string message)
    {
        public DateTime Created { get; set; } = DateTime.Now;
    }
}