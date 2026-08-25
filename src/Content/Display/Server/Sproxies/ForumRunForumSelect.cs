using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunForumSelect
{
    public static async Task<IList<Forum>> ExecuteAll(String connection, Guid? sessionId, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunForumSelectAll" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@LicenseIds", licenseIds);
        return await command.ReadAll(connection, ForumRunDataReader.ReadForum);
    }

    public static async Task<Forum?> Execute(String connection, Guid? sessionId, Guid? id, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunForumSelect" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@LicenseIds", licenseIds);
        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync()) return null;

            var forum = ForumRunDataReader.ReadForum(reader);

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                var bundleId = reader.ReadGuid(0);
                var bundle = forum.ForumBundles.FirstOrDefault(x => x.BundleId == bundleId);
                if (bundle is null)
                {
                    bundle = ForumRunDataReader.ReadForumBundle(reader);
                    forum.ForumBundles.Add(bundle);
                }

                var tagId = reader.ReadGuid(4);
                if (tagId.HasValue && bundle.Tags.All(x => x.Id != tagId))
                    bundle.Tags.Add(ForumRunDataReader.ReadTag(reader, 4, false));
            }

            return forum;
        });
    }
}