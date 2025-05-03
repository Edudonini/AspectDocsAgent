

namespace AspectDocsAgent.Domain
{
    public sealed class DocumentChunk
    {
        public required string Id { get; init; }
        public required string Content { get; init; }
        public required Embedding Emb { get; init;  }
    }
}
