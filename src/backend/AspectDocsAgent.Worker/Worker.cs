using AspectDocsAgent.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public sealed class IngestionWorker : BackgroundService
{
    private readonly IngestPdfUseCase _ingest;
    private readonly string _watchDir;

    public IngestionWorker(IngestPdfUseCase ingest, IConfiguration cfg)
    {
        _ingest = ingest;
        _watchDir = cfg["DocsDir"] ?? "./docs";   // fallback
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!Directory.Exists(_watchDir))
            Directory.CreateDirectory(_watchDir);

        var fsw = new FileSystemWatcher(_watchDir, "*.pdf")
        {
            EnableRaisingEvents = true,
            IncludeSubdirectories = false
        };

        fsw.Created += async (_, e) =>
        {
            Console.WriteLine($"[Worker] Novo PDF detectado: {e.FullPath}");
            try { await _ingest.ExecuteAsync(e.FullPath); }
            catch (Exception ex)
            { Console.WriteLine($"[Worker] Erro na ingestão: {ex.Message}"); }
        };

        Console.WriteLine($"[Worker] Monitorando {_watchDir}");
        return Task.CompletedTask;   // fica vivo enquanto o host vive
    }
}
