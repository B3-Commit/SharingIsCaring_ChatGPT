namespace SharingIsCaring.Core.Models.OpenAI;

public class OpenAIClientServiceOptions
{
    public string ServiceUri { get; set; } = null!;
    public string ServiceKey { get; set; } = null!;

    public IEnumerable<Deployment> Deployments { get; set; }
        = Enumerable.Empty<Deployment>();

    public class Deployment
    {
        public string Name { get; set; } = null!;
        public string? ModelName { get; set; }
    }
}