namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Shirt shirt)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirt.Id);
        command.AddParameter("@Name", 120, shirt.Name);
        command.AddParameter("@BrandId", shirt.BrandId);
        command.AddParameter("@Fit", (Int32?)shirt.Fit);
        command.AddParameter("@Material", 80, shirt.Material);
        command.AddParameter("@Price", shirt.Price);
        command.AddParameter("@HeroImageId", shirt.HeroImageFile.Id);
        command.AddParameter("@GuidePdfId", shirt.GuidePdfFile.Id);

        await command.Execute(connection, transaction);
    }
}