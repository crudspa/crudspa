namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@PortalId", segment.PortalId);
        command.AddParameter("@ParentId", segment.ParentId);
        command.AddParameter("@Key", 100, segment.Key);
        command.AddParameter("@StatusId", segment.StatusId);
        command.AddParameter("@Title", 150, segment.Title);
        command.AddParameter("@PermissionId", segment.PermissionId);
        command.AddParameter("@IconId", segment.IconId);
        command.AddParameter("@Fixed", segment.Fixed);
        command.AddParameter("@RequiresId", segment.RequiresId);
        command.AddParameter("@Recursive", segment.Recursive ?? false);
        command.AddParameter("@TypeId", segment.TypeId);
        command.AddParameter("@AllLicenses", segment.AllLicenses ?? true);
        command.AddParameter("@Licenses", segment.Licenses);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}