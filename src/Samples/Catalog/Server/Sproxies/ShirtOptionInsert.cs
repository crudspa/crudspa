namespace Crudspa.Samples.Catalog.Server.Sproxies;

public static class ShirtOptionInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ShirtOption shirtOption)
    {
        await using var command = new SqlCommand();
        command.CommandText = "SamplesCatalog.ShirtOptionInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ShirtId", shirtOption.ShirtId);
        command.AddParameter("@SkuBase", 40, shirtOption.SkuBase);
        command.AddParameter("@Price", shirtOption.Price);
        command.AddParameter("@ColorId", shirtOption.ColorId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}