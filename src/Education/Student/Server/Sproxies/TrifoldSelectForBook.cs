namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldSelectForBook
{
    public static async Task<IList<Trifold>> Execute(String connection, Guid? bookId, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldSelectForBook";

        command.AddParameter("@BookId", bookId);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadAll(connection, ReadTrifold);
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
            PageCount = reader.ReadInt32(5),
        };
    }
}