using AspectDocsAgent.Application.Contracts;
using AspectDocsAgent.Domain;
using System.Diagnostics;

namespace AspectDocsAgent.Application;

public sealed class AskQuestionUseCase
{
    private readonly IEmbeddingService _embed;
    private readonly IChunkRepository _repo;
    private readonly IChatCompletionService _chat;

    public AskQuestionUseCase(IEmbeddingService embed,
                              IChunkRepository repo,
                              IChatCompletionService chat)
    {
        _embed = embed;
        _repo = repo;
        _chat = chat;
    }

    public async Task<string> ExecuteAsync(string question, int k = 10)
    {
        var qEmb = await _embed.EmbedAsync(question);
        var scored = new List<(string text, double sim)>();

        await foreach (var c in _repo.GetAllAsync())
            scored.Add((c.Content, Cosine(qEmb.Vector, c.Emb.Vector)));

        var context = string.Join("\n---\n",
                       scored.OrderByDescending(s => s.sim)
                             .Take(Math.Max(1, k))               // garante ao menos 1
                             .Select(s => s.text));
        
        Debug.WriteLine("CTX:\n" + context);   // ou veja no Watch
        var system = $"""
        Você é um especialista no sistema Aspect.
        Use APENAS o CONTEXTO abaixo para responder em português.
        Se não souber, informe que a documentação não cobre o assunto.
        ### CONTEXTO
        {context}
        ### FIM
        """;

        return await _chat.GetCompletionAsync(system, question);
    }

    private static double Cosine(float[] a, float[] b)
    {
        double dot = 0, na = 0, nb = 0;
        for (int i = 0; i < a.Length; i++)
        { dot += a[i] * b[i]; na += a[i] * a[i]; nb += b[i] * b[i]; }
        return dot / (Math.Sqrt(na) * Math.Sqrt(nb) + 1e-8);
    }
}
