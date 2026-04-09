namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtSelect
{
    public static async Task<Shirt?> Execute(String connection, Guid? sessionId, Shirt shirt)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirt.Id);

        return await command.ReadSingle(connection, ReadShirt);
    }

    private static Shirt ReadShirt(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            BrandId = reader.ReadGuid(2),
            BrandName = reader.ReadString(3),
            Fit = reader.ReadEnum<Shirt.Fits>(4),
            Material = reader.ReadString(5),
            Price = reader.ReadSingle(6),
            HeroImageFile = new()
            {
                Id = reader.ReadGuid(7),
                BlobId = reader.ReadGuid(8),
                Name = reader.ReadString(9),
                Format = reader.ReadString(10),
                Width = reader.ReadInt32(11),
                Height = reader.ReadInt32(12),
                Caption = reader.ReadString(13),
            },
            GuidePdfFile = new()
            {
                Id = reader.ReadGuid(14),
                BlobId = reader.ReadGuid(15),
                Name = reader.ReadString(16),
                Format = reader.ReadString(17),
                Description = reader.ReadString(18),
            },
            ShirtOptionCount = reader.ReadInt32(19),
        };
    }
}