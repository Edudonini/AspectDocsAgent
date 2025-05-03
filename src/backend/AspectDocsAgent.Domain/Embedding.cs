
namespace AspectDocsAgent.Domain
{
    public sealed record Embedding(float[] Vector)
    {
        public const int Dimensions = 768;
    }
}
