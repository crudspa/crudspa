namespace Crudspa.Samples.Composer.Server.Sproxies;

using Composer = Shared.Contracts.Data.Composer;

public static class ComposerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Composer composer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", composer.Id);

        await command.Execute(connection, transaction);
    }
}