using AspectDocsAgent.Application;
using AspectDocsAgent.Application.Contracts;
using AspectDocsAgent.Infrastructure;
using Microsoft.AspNetCore.Builder;

namespace AspectDocsAgent.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // DI
        builder.Services.AddSingleton<IEmbeddingService, OllamaEmbeddingService>();
        builder.Services.AddSingleton<IChatCompletionService, OllamaChatCompletionService>();
        builder.Services.AddSingleton<IChunkRepository>(_ => new SqliteChunkRepository());
        builder.Services.AddSingleton<IChatCompletionStreamable, OllamaChatStreamService>();
        builder.Services.AddSingleton<IngestPdfUseCase>();
        builder.Services.AddSingleton<AskQuestionUseCase>();
        builder.Services.AddSingleton<StreamQuestionUseCase>();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddCors(opt =>
        {
            opt.AddPolicy("dev", p => p
                .WithOrigins("http://localhost:4200")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
        });


        var app = builder.Build();
        app.UseSwagger(); app.UseSwaggerUI();
        app.UseCors("dev");

        // endpoints
        app.MapPost("/api/ingest",
            async (IngestRequest req, IngestPdfUseCase uc) =>
            {
                foreach (var pdf in Directory.EnumerateFiles(req.Directory, "*.pdf"))
                    await uc.ExecuteAsync(pdf);
                return Results.Ok("PDFs ingeridos");
            });

        app.MapPost("/api/chat",
            async (ChatRequest req, AskQuestionUseCase uc) =>
                Results.Ok(await uc.ExecuteAsync(req.Question)));

        app.MapGet("/api/chat/stream", async (string q,
            StreamQuestionUseCase uc, HttpResponse res, CancellationToken ct) =>
        {
            res.Headers.ContentType = "text/event-stream";

            await foreach (var token in uc.ExecuteAsync(q, ct))
            {
                await res.WriteAsync($"data: {token}\n\n", ct);
                await res.Body.FlushAsync(ct);
            }
        });

        app.Run();
    }

    public record IngestRequest(string Directory);
    public record ChatRequest(string Question);
}
