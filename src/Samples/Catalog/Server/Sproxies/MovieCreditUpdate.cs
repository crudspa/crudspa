namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieCreditUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MovieCredit movieCredit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieCreditUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", movieCredit.Id);
        command.AddParameter("@Name", 120, movieCredit.Name);
        command.AddParameter("@Part", 120, movieCredit.Part);
        command.AddParameter("@Billing", movieCredit.Billing);

        await command.Execute(connection, transaction);
    }
}