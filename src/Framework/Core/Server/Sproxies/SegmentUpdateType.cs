namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentUpdateType
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentUpdateType";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segment.Id);
        command.AddParameter("@TypeId", segment.TypeId);
        command.AddParameter("@ConfigJson", segment.ConfigJson);

        await command.Execute(connection, transaction);
    }
}