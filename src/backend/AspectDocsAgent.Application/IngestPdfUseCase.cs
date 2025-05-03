using AspectDocsAgent.Domain;
using AspectDocsAgent.Application.Contracts;
using System.Text;

namespace AspectDocsAgent.Application;

public sealed class IngestPdfUseCase
{
    private readonly IEmbeddingService _embed;
    private readonly IChunkRepository _repo;

    public IngestPdfUseCase(IEmbeddingService embed, IChunkRepository repo)
    {
        _embed = embed;
        _repo = repo;
    }

    public async Task ExecuteAsync(string pdfPath, int chunkSize = 400)
    {
        var text = PdfHelper.ExtractText(pdfPath);
        var chunks = Split(text, chunkSize).ToArray();

        for (int i = 0; i < chunks.Length; i++)
        {
            var id = $"{Path.GetFileName(pdfPath)}_ck{i}";
            var emb = await _embed.EmbedAsync(chunks[i]);

            await _repo.SaveAsync(new DocumentChunk
            {
                Id = id,
                Content = chunks[i],
                Emb = emb
            });
        }
    }

    private static IEnumerable<string> Split(string text, int size)
    {
        var words = text.Split(' ');
        var sb = new StringBuilder();
        foreach (var w in words)
        {
            if (sb.Length + w.Length + 1 > size)
            {
                yield return sb.ToString();
                sb.Clear();
            }
            sb.Append(w).Append(' ');
        }
        if (sb.Length > 0) yield return sb.ToString();
    }
}
