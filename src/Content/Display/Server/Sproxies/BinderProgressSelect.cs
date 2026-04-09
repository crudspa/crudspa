namespace Crudspa.Content.Display.Server.Sproxies;

public static class BinderProgressSelect
{
    public static async Task<BinderProgress> Execute(String connection, Guid? sessionId, Guid? binderId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.BinderProgressSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BinderId", binderId);

        var progress = await command.ReadSingle(connection, ReadBinderProgress);

        return progress ?? new BinderProgress
        {
            BinderId = binderId,
            TimesCompleted = 0,
        };
    }

    public static BinderProgress ReadBinderProgress(SqlDataReader reader)
    {
        return new()
        {
            ContactId = reader.ReadGuid(0),
            BinderId = reader.ReadGuid(1),
            TimesCompleted = reader.ReadInt32(2),
        };
    }
}