namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitBookSelectForUnit
{
    public static async Task<IList<UnitBook>> Execute(String connection, Guid? sessionId, Guid? unitId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitBookSelectForUnit";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@UnitId", unitId);

        return await command.ReadAll(connection, ReadUnitBook);
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