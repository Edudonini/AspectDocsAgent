using AspectDocsAgent.Application.Contracts;
using System.Runtime.CompilerServices;
using System.Text;

namespace AspectDocsAgent.Application;

public sealed class StreamQuestionUseCase
{
    private readonly IEmbeddingService _embed;
    private readonly IChunkRepository _repo;
    private readonly IChatCompletionStreamable _chat;

    public StreamQuestionUseCase(IEmbeddingService embed,
                                 IChunkRepository repo,
                                 IChatCompletionStreamable chat)
    {
        _embed = embed; _repo = repo; _chat = chat;
    }

    public async IAsyncEnumerable<string> ExecuteAsync(
        string question,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        // ===== 1) Montar contexto =====
        var qEmb = await _embed.EmbedAsync(question);
        var all = new List<(string content, double sim)>();

        await foreach (var c in _repo.GetAllAsync())
            all.Add((c.Content, Cosine(qEmb.Vector, c.Emb.Vector)));

        var context = string.Join("\n---\n",
                       all.OrderByDescending(x => x.sim)
                          .Take(5)
                          .Select(x => x.content));

        var system = $"""
        Você é um especialista no sistema Aspect.
        Responda usando APENAS o CONTEXTO. Se não encontrar, diga que não sabe.
        ### CONTEXTO
        {context}
        ### FIM
        """;

        // ===== 2) Stream do modelo =====
        await foreach (var tok in _chat.StreamCompletionAsync(system, question, ct))
            yield return tok;
    }

    private static double Cosine(float[] a, float[] b)
    {
        double dot = 0, na = 0, nb = 0;
        for (int i = 0; i < a.Length; i++) { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        return dot / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-8);
    }
}
