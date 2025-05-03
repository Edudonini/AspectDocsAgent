using Xunit;
using FluentAssertions;
using AspectDocsAgent.Application;
using AspectDocsAgent.Domain;
using AspectDocsAgent.Application.Contracts;
using System.Collections.Generic;
using System.Linq;

public sealed class AskQuestionUseCaseTests
{
    [Fact]
    public async Task Returns_error_when_context_empty()
    {
        // dummies
        var embed = new DummyEmbed();
        var repo = new DummyRepo();
        var chat = new DummyChat();

        var uc = new AskQuestionUseCase(embed, repo, chat);
        var answer = await uc.ExecuteAsync("Como configuro trunk?");

        answer.Should().Contain("não sabe");
    }

    private sealed class DummyEmbed : IEmbeddingService
    {
        public Task<Embedding> EmbedAsync(string t) =>
            Task.FromResult(new Embedding(new float[Embedding.Dimensions]));
    }
    private sealed class DummyRepo : IChunkRepository
    {
        public IAsyncEnumerable<DocumentChunk> GetAllAsync() =>
            AsyncEnumerable.Empty<DocumentChunk>();
        public Task SaveAsync(DocumentChunk _) => Task.CompletedTask;
    }
    private sealed class DummyChat : IChatCompletionService
    {
        public Task<string> GetCompletionAsync(string s, string u) =>
            Task.FromResult("Desculpe, não sabe.");
    }
}
