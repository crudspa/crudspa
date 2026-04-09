namespace Crudspa.Samples.Composer.Server.Sproxies;

using Composer = Shared.Contracts.Data.Composer;

public static class ComposerSelect
{
    public static async Task<Composer?> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesComposer.ComposerSelect";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadComposer);
    }

    private static Composer ReadComposer(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            OrganizationId = reader.ReadGuid(1),
        };
    }
}