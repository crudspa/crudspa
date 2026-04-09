namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtSelectWhere
{
    public static async Task<IList<Shirt>> Execute(String connection, Guid? sessionId, ShirtSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtSelectWhere";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Brands", search.Brands);

        return await command.ReadAll(connection, ReadShirt);
    }

    private static Shirt ReadShirt(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
            Name = reader.ReadString(3),
            HeroImageFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
            BrandId = reader.ReadGuid(11),
            BrandName = reader.ReadString(12),
            Fit = reader.ReadEnum<Shirt.Fits>(13),
            Material = reader.ReadString(14),
            Price = reader.ReadSingle(15),
            ShirtOptionCount = reader.ReadInt32(16),
        };
    }
}