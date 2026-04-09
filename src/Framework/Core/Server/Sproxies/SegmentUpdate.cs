namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", segment.Id);
        command.AddParameter("@Key", 100, segment.Key);
        command.AddParameter("@StatusId", segment.StatusId);
        command.AddParameter("@Title", 150, segment.Title);
        command.AddParameter("@PermissionId", segment.PermissionId);
        command.AddParameter("@IconId", segment.IconId);
        command.AddParameter("@Fixed", segment.Fixed);
        command.AddParameter("@RequiresId", segment.RequiresId);
        command.AddParameter("@Recursive", segment.Recursive ?? false);
        command.AddParameter("@AllLicenses", segment.AllLicenses ?? true);
        command.AddParameter("@Licenses", segment.Licenses);

        await command.Execute(connection, transaction);
    }
}