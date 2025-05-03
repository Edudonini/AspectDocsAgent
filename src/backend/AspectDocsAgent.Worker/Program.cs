// top‑level program (NET 8 Worker)
using AspectDocsAgent.Application;
using AspectDocsAgent.Application.Contracts;
using AspectDocsAgent.Infrastructure;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // -- Serviços de infraestrutura --
        services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
        services.AddSingleton<IChatCompletionService, OllamaChatCompletionService>();
        services.AddSingleton<IChunkRepository>(_ => new SqliteChunkRepository());

        // -- Casos de uso da camada Application --
        services.AddSingleton<IngestPdfUseCase>();
        services.AddSingleton<AskQuestionUseCase>();

        // -- Hosted Service que observa a pasta de PDFs --
        services.AddHostedService<IngestionWorker>();
    })
    .Build();

await host.RunAsync();
