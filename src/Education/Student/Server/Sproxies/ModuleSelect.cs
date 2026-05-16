namespace Crudspa.Education.Student.Server.Sproxies;

public static class ModuleSelect
{
    public static async Task<Module?> Execute(String connection, Module module, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.ModuleSelect";

        command.AddParameter("@Id", module.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var module = ReadModule(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                module.GuideBinder.Pages.Add(GuideDataReaders.ReadGuidePage(reader));

            return module;
        });
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
            GuideBinder = GuideDataReaders.ReadGuideBinder(reader, 8),
            StatusName = reader.ReadString(18),
            BinderDisplayView = reader.ReadString(19),
        };
    }
}