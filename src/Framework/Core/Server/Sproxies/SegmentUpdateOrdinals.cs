namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentUpdateOrdinals
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentUpdateOrdinals";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Orderables", orderables);

        await command.Execute(connection, transaction);
    }
}