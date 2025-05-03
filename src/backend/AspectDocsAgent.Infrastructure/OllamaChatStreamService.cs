using System.Net.Http.Json;
using AspectDocsAgent.Application.Contracts;
using System.Runtime.CompilerServices;

namespace AspectDocsAgent.Infrastructure;

public sealed class OllamaChatStreamService : IChatCompletionStreamable
{
    private const string ModelName = "tinyllama:latest";

    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:11434"),
        Timeout = Timeout.InfiniteTimeSpan
    };

    public async IAsyncEnumerable<string> StreamCompletionAsync(
        string systemPrompt,
        string userPrompt,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var resp = await _http.PostAsJsonAsync("/api/chat", new
        {
            model = ModelName,
            stream = true,
            options = new { num_ctx = 2048 },
            messages = new[]
            {
                new { role = "system", content = systemPrompt },
                new { role = "user",   content = userPrompt   }
            }
        }, ct);

        resp.EnsureSuccessStatusCode();

        using var stream = await resp.Content.ReadAsStreamAsync(ct);
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            if (!line.StartsWith("data: ")) continue;
            var payload = line[6..];
            if (payload == "[DONE]") yield break;

            var json = System.Text.Json.JsonDocument.Parse(payload).RootElement;

            string? token = null;
            if (json.TryGetProperty("message", out var msgProp) &&
                msgProp.TryGetProperty("content", out var mc))
            {
                token = mc.GetString();
            }
            else if (json.TryGetProperty("response", out var rp))
            {
                token = rp.GetString();
            }

            if (!string.IsNullOrEmpty(token))
                yield return token;
        }
    }
}
