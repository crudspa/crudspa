namespace Crudspa.Content.Display.Server.Sproxies;

public static class TrackContentIsAccessible
{
    public static async Task<Boolean> Execute(
        String connection,
        IEnumerable<Guid?> licenseIds,
        Guid? trackId = null,
        Guid? courseId = null,
        Guid? binderId = null,
        Guid? pageId = null,
        Guid? elementId = null)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.TrackContentIsAccessible";

        command.AddParameter("@LicenseIds", licenseIds);
        command.AddParameter("@TrackId", trackId);
        command.AddParameter("@CourseId", courseId);
        command.AddParameter("@BinderId", binderId);
        command.AddParameter("@PageId", pageId);
        command.AddParameter("@ElementId", elementId);

        var output = command.AddOutputParameter("@IsAccessible", System.Data.SqlDbType.Bit);
        await command.Execute(connection);
        return output.Value is true;
    }
}