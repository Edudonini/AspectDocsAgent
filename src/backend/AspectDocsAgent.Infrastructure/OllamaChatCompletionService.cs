using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using AspectDocsAgent.Application.Contracts;

namespace AspectDocsAgent.Infrastructure;

public sealed class OllamaChatCompletionService : IChatCompletionService
{
    private const string ModelName = "tinyllama:latest";

    private readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:11434"),
        Timeout = Timeout.InfiniteTimeSpan
    };

    public async Task<string> GetCompletionAsync(string systemPrompt, string userPrompt)
    {
        Debug.WriteLine(systemPrompt.Substring(0, 200));   // primeiro trecho
        Debug.WriteLine(userPrompt);
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
        });

        resp.EnsureSuccessStatusCode();

        using var stream = await resp.Content.ReadAsStreamAsync();
        using var reader = new StreamReader(stream);
        var answer = new StringBuilder();

        string? line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {

            if (!line.StartsWith("data: ")) continue;
            var payload = line[6..];
            if (payload == "[DONE]") break;

            var json = System.Text.Json.JsonDocument.Parse(payload).RootElement;

            // TinyLlama envia OR message.content OR response […]
            string? token = null;
            if (!string.IsNullOrEmpty(token))
            {
                Debug.WriteLine("TOKEN: " + token);
                answer.Append(token);
            }

            if (json.TryGetProperty("message", out var msgProp) &&
                msgProp.TryGetProperty("content", out var mc))
            {
                token = mc.GetString();
            }
            else if (json.TryGetProperty("response", out var respProp))
            {
                token = respProp.GetString();
            }

            if (!string.IsNullOrEmpty(token))
                answer.Append(token);
        }

        return answer.Length > 0 ? answer.ToString()
                                 : "Desculpe, não encontrei essa informação no contexto.";
    }
}
