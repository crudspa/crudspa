namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentUpdateParent
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentUpdateParent";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segment.Id);
        command.AddParameter("@PortalId", segment.PortalId);
        command.AddParameter("@ParentId", segment.ParentId);

        await command.Execute(connection, transaction);
    }
}