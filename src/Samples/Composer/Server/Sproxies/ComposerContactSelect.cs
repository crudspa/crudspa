namespace Crudspa.Samples.Composer.Server.Sproxies;

public static class ComposerContactSelect
{
    public static async Task<ComposerContact?> Execute(String connection, Guid? sessionId, ComposerContact composerContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerContactSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", composerContact.Id);

        return await command.ReadSingle(connection, ReadComposerContact);
    }

    private static ComposerContact ReadComposerContact(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            UserId = reader.ReadGuid(1),
            ContactId = reader.ReadGuid(2),
        };
    }
}