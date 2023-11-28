using SharingIsCaring.Core.Models.OpenAI;

namespace SharingIsCaring.Console.Models;

public class ConsoleSettings
{
    public OpenAIClientServiceOptions OpenAI { get; set; } = null!;
}