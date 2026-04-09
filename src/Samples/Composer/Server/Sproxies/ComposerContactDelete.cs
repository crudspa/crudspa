namespace Crudspa.Samples.Composer.Server.Sproxies;

public static class ComposerContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ComposerContact composerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", composerContact.Id);

        await command.Execute(connection, transaction);
    }
}