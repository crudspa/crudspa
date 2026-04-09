namespace Crudspa.Education.Student.Server.Sproxies;

public static class TrifoldSelect
{
    public static async Task<Trifold?> Execute(String connection, Trifold trifold, Guid? sessionId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationStudent.TrifoldSelect";

        command.AddParameter("@Id", trifold.Id);
        command.AddParameter("@SessionId", sessionId);

        return await command.ReadSingle(connection, ReadTrifold);
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
            GuideImage = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                Width = reader.ReadInt32(9),
                Height = reader.ReadInt32(10),
                Caption = reader.ReadString(11),
            },
            UnitId = reader.ReadGuid(12),
            BinderDisplayView = reader.ReadString(13),
        };
    }
}