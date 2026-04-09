namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class MovieCreditInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MovieCredit movieCredit)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.MovieCreditInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@MovieId", movieCredit.MovieId);
        command.AddParameter("@Name", 120, movieCredit.Name);
        command.AddParameter("@Part", 120, movieCredit.Part);
        command.AddParameter("@Billing", movieCredit.Billing);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}