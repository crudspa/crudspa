namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieCreditDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MovieCredit movieCredit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieCreditDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", movieCredit.Id);

        await command.Execute(connection, transaction);
    }
}