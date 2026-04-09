namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ModuleSelectForBook
{
    public static async Task<IList<Module>> Execute(String connection, Guid? sessionId, Guid? bookId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ModuleSelectForBook";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", bookId);

        return await command.ReadAll(connection, ReadModule);
    }

    private static Module ReadModule(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            BookKey = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            IconId = reader.ReadGuid(6),
            IconCssClass = reader.ReadString(7),
            RequiresAchievementId = reader.ReadGuid(8),
            RequiresAchievementTitle = reader.ReadString(9),
            GeneratesAchievementId = reader.ReadGuid(10),
            GeneratesAchievementTitle = reader.ReadString(11),
            Ordinal = reader.ReadInt32(12),
            Binder = new()
            {
                Id = reader.ReadGuid(13),
                TypeId = reader.ReadGuid(14),
                TypeName = reader.ReadString(15),
            },
        };
    }
}