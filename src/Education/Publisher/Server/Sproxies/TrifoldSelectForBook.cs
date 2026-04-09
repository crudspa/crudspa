namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class TrifoldSelectForBook
{
    public static async Task<IList<Trifold>> Execute(String connection, Guid? sessionId, Guid? bookId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.TrifoldSelectForBook";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@BookId", bookId);

        return await command.ReadAll(connection, ReadTrifold);
    }

    private static Trifold ReadTrifold(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            BookId = reader.ReadGuid(1),
            BookKey = reader.ReadString(2),
            Title = reader.ReadString(3),
            StatusId = reader.ReadGuid(4),
            StatusName = reader.ReadString(5),
            RequiresAchievementId = reader.ReadGuid(6),
            RequiresAchievementTitle = reader.ReadString(7),
            GeneratesAchievementId = reader.ReadGuid(8),
            GeneratesAchievementTitle = reader.ReadString(9),
            Ordinal = reader.ReadInt32(10),
            Binder = new()
            {
                Id = reader.ReadGuid(11),
                TypeId = reader.ReadGuid(12),
                TypeName = reader.ReadString(13),
            },
        };
    }
}