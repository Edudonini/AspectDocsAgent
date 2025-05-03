using AspectDocsAgent.Application.Contracts;
using AspectDocsAgent.Domain;
using Microsoft.Data.Sqlite;

namespace AspectDocsAgent.Infrastructure;

public sealed class SqliteChunkRepository : IChunkRepository
{
    private readonly string _cs;

    public SqliteChunkRepository(string file = "embeddings.db")
    {
        var csb = new SqliteConnectionStringBuilder { DataSource = file };
        _cs = csb.ConnectionString;
        EnsureTable();
    }

    public async Task SaveAsync(DocumentChunk chunk)
    {
        using var con = new SqliteConnection(_cs);
        await con.OpenAsync();
        var cmd = con.CreateCommand();
        cmd.CommandText = """
        INSERT OR REPLACE INTO Embeddings(Id,Content,Vector)
        VALUES ($id,$content,$vec)
        """;
        cmd.Parameters.AddWithValue("$id", chunk.Id);
        cmd.Parameters.AddWithValue("$content", chunk.Content);
        cmd.Parameters.AddWithValue("$vec", string.Join(',', chunk.Emb.Vector));
        await cmd.ExecuteNonQueryAsync();
    }

    public async IAsyncEnumerable<DocumentChunk> GetAllAsync()
    {
        using var con = new SqliteConnection(_cs);
        await con.OpenAsync();
        var cmd = con.CreateCommand();
        cmd.CommandText = "SELECT Id, Content, Vector FROM Embeddings";
        using var rd = await cmd.ExecuteReaderAsync();
        while (await rd.ReadAsync())
        {
            var vec = rd.GetString(2).Split(',').Select(float.Parse).ToArray();
            yield return new DocumentChunk
            {
                Id = rd.GetString(0),
                Content = rd.GetString(1),
                Emb = new Embedding(vec)
            };
        }
    }

    private void EnsureTable()
    {
        using var con = new SqliteConnection(_cs);
        con.Open();
        var cmd = con.CreateCommand();
        cmd.CommandText = """
        CREATE TABLE IF NOT EXISTS Embeddings(
            Id      TEXT PRIMARY KEY,
            Content TEXT NOT NULL,
            Vector  TEXT NOT NULL
        );
        """;
        cmd.ExecuteNonQuery();
    }
}
