namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Movie movie)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", movie.Id);

        await command.Execute(connection, transaction);
    }
}