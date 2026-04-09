namespace Crudspa.Content.Design.Server.Sproxies;

public static class MultimediaItemUpdateOrdinalsByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MultimediaItemUpdateOrdinalsByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Orderables", orderables);

        await command.Execute(connection, transaction);
    }
}