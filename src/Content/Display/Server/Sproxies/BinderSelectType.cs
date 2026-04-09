namespace Crudspa.Content.Display.Server.Sproxies;

public static class BinderSelectType
{
    public static async Task<BinderTypeFull?> Execute(String connection, Guid? binderId)
    {
        await using var command = new SqlCommand();

        command.CommandText = "ContentDisplay.BinderSelectType";

        command.AddParameter("@BinderId", binderId);

        return await command.ReadSingle(connection, ReadBinderType);
    }

    private static BinderTypeFull ReadBinderType(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            DesignView = reader.ReadString(2),
            DisplayView = reader.ReadString(3),
        };
    }
}