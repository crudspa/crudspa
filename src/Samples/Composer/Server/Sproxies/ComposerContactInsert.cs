namespace Crudspa.Samples.Composer.Server.Sproxies;

public static class ComposerContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ComposerContact composerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ContactId", composerContact.ContactId);
        command.AddParameter("@UserId", composerContact.UserId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}