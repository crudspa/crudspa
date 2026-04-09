namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtOptionUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ShirtOption shirtOption)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtOptionUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", shirtOption.Id);
        command.AddParameter("@SkuBase", 40, shirtOption.SkuBase);
        command.AddParameter("@Price", shirtOption.Price);
        command.AddParameter("@ColorId", shirtOption.ColorId);

        await command.Execute(connection, transaction);
    }
}