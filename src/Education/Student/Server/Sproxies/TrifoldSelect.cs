namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldSelect
{
    public static async Task<Trifold?> Execute(String connection, Trifold trifold, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldSelect";

        command.AddParameter("@Id", trifold.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var trifold = ReadTrifold(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                trifold.GuideBinder.Pages.Add(GuideDataReaders.ReadGuidePage(reader));

            return trifold;
        });
    }

    private static Trifold ReadTrifold(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Title = reader.ReadString(1),
            BookId = reader.ReadGuid(2),
            BinderId = reader.ReadGuid(3),
            Ordinal = reader.ReadInt32(4),
            GuideBinder = GuideDataReaders.ReadGuideBinder(reader, 5),
            UnitId = reader.ReadGuid(15),
            BinderDisplayView = reader.ReadString(16),
        };
    }
}