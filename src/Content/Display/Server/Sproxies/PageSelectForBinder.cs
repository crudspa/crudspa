namespace Crudspa.Content.Display.Server.Sproxies;

public static class PageSelectForBinder
{
    public static async Task<IList<Page>> Execute(String connection, Guid? binderId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.PageSelectForBinder";

        command.AddParameter("@BinderId", binderId);

        return await command.ReadAll(connection, ReadPage);
    }

    private static Page ReadPage(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BinderId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            GuideText = reader.ReadString(3),
            Ordinal = reader.ReadInt32(4),
            StatusName = reader.ReadString(5),
            SectionCount = reader.ReadInt32(6),
        };
    }
}