
using AspectDocsAgent.Domain;

namespace AspectDocsAgent.Application.Contracts
{
    public interface IChunkRepository
    {
        Task SaveAsync(DocumentChunk chunk);
        IAsyncEnumerable<DocumentChunk> GetAllAsync();
    }
}
