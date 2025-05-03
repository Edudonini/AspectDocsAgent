

namespace AspectDocsAgent.Application.Contracts
{
    public interface IChatCompletionService
    {
        Task<string> GetCompletionAsync(string systemPrompt, string userPrompt);

      
    }
}
