namespace Crudspa.Content.Design.Server.Sproxies;

public static class ButtonSelect
{
    public static async Task<Button?> Execute(String connection, Guid? sessionId, Guid? id)
    {
        if (!id.HasValue)
            return null;

        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ButtonSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);

        return await command.ReadSingle(connection, ReadButton);
    }

    private static Button ReadButton(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ImageFile = new()
            {
                Id = reader.ReadGuid(1),
                BlobId = reader.ReadGuid(2),
                Name = reader.ReadString(3),
                Format = reader.ReadString(4),
                Width = reader.ReadInt32(5),
                Height = reader.ReadInt32(6),
                Caption = reader.ReadString(7),
            },
        };
    }
}