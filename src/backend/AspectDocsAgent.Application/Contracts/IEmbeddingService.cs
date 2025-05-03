

using AspectDocsAgent.Domain;

namespace AspectDocsAgent.Application.Contracts
{
    public interface IEmbeddingService
    {
        Task<Embedding> EmbedAsync(string text);
    }
}
