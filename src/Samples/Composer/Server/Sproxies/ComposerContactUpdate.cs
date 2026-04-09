namespace Crudspa.Samples.Composer.Server.Sproxies;

public static class ComposerContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ComposerContact composerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", composerContact.Id);
        command.AddParameter("@UserId", composerContact.UserId);

        await command.Execute(connection, transaction);
    }
}