using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspectDocsAgent.Application.Contracts
{
    public interface IChatCompletionStreamable
    {
        IAsyncEnumerable<string> StreamCompletionAsync(string systemPrompt,
                                                        string userPrompt,
                                                        CancellationToken ct = default);
    }
}
