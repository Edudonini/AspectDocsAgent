using System.Net.Http.Json;
using AspectDocsAgent.Application.Contracts;
using AspectDocsAgent.Domain;

namespace AspectDocsAgent.Infrastructure;

public sealed class OllamaEmbeddingService : IEmbeddingService
{
    private readonly HttpClient _http = new() { BaseAddress = new Uri("http://localhost:11434") };

    public async Task<Embedding> EmbedAsync(string text)
    {
        var resp = await _http.PostAsJsonAsync("/api/embeddings", new
        {
            model = "nomic-embed-text",
            prompt = text
        });
        resp.EnsureSuccessStatusCode();
        var data = await resp.Content.ReadFromJsonAsync<Resp>();
        return new Embedding(data!.Embedding);
    }

    private record Resp(float[] Embedding);
}
