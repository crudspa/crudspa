namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentKeyIsUnique
{
    public static async Task<Boolean> Execute(String connection, Segment segment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentKeyIsUnique";

        command.AddParameter("@Id", segment.Id);
        command.AddParameter("@Key", 100, segment.Key);
        command.AddParameter("@PortalId", segment.PortalId);
        command.AddParameter("@ParentId", segment.ParentId);

        return await command.ExecuteBoolean(connection, "@Unique");
    }
}