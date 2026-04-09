namespace Crudspa.Framework.Core.Server.Sproxies;

public static class SegmentTypeSelectFull
{
    public static async Task<IList<SegmentTypeFull>> Execute(String connection, Guid? portalId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "FrameworkCore.SegmentTypeSelectFull";

        command.AddParameter("@PortalId", portalId);

        return await command.ReadAll(connection, ReadSegmentType);
    }

    private static SegmentTypeFull ReadSegmentType(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            EditorView = reader.ReadString(2),
            DisplayView = reader.ReadString(3),
            Ordinal = reader.ReadInt32(4),
        };
    }
}