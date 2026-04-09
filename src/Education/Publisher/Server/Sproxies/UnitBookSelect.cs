namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitBookSelect
{
    public static async Task<UnitBook?> Execute(String connection, Guid? sessionId, UnitBook unitBook)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitBookSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitBook.Id);

        return await command.ReadSingle(connection, ReadUnitBook);
    }

    private static UnitBook ReadUnitBook(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            UnitId = reader.ReadGuid(1),
            BookId = reader.ReadGuid(2),
            Treatment = reader.ReadBoolean(3),
            Control = reader.ReadBoolean(4),
            Ordinal = reader.ReadInt32(5),
            BookKey = reader.ReadString(6),
            BookTitle = reader.ReadString(7),
            BookCoverImage = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                Width = reader.ReadInt32(12),
                Height = reader.ReadInt32(13),
                Caption = reader.ReadString(14),
            },
        };
    }
}