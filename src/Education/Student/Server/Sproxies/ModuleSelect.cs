namespace Crudspa.Education.Student.Server.Sproxies;

public static class ModuleSelect
{
    public static async Task<Module?> Execute(String connection, Module module, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ModuleSelect";

        command.AddParameter("@Id", module.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadModule);
    }

    private static Module ReadModule(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            IconName = reader.ReadString(2),
            BookId = reader.ReadGuid(3),
            StatusId = reader.ReadGuid(4),
            BinderId = reader.ReadGuid(5),
            Ordinal = reader.ReadInt32(6),
            BookTitle = reader.ReadString(7),
            BookGuideImage = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                Width = reader.ReadInt32(12),
                Height = reader.ReadInt32(13),
                Caption = reader.ReadString(14),
            },
            StatusName = reader.ReadString(15),
            BinderDisplayView = reader.ReadString(16),
        };
    }
}