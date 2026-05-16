namespace Crudspa.Framework.Core.Server.Sproxies;

public static class ContactSelectNames
{
    public static async Task<IList<Named>> Execute(String connection, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.ContactSelectNames";

        command.AddParameter("@SessionId", sessionId);

        return await command.ReadNameds(connection);
    }
}