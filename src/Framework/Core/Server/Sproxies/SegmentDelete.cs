namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segment.Id);

        await command.Execute(connection, transaction);
    }
}