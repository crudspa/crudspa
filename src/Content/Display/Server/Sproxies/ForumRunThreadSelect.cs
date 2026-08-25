using Thread = Crudspa.Content.Display.Shared.Contracts.Data.Thread;

using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Display.Server.Sproxies;

public static class ForumRunThreadSelect
{
    public static async Task<IList<Thread>> ExecuteSearch(String connection, Guid? sessionId, ThreadSearch search, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunThreadSearch" };

        search.PostedRange.ResolveDates(search.TimeZoneId ?? Constants.DefaultTimeZone);

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@PostedStart", search.PostedRange.StartDateTimeOffset);
        command.AddParameter("@PostedEnd", search.PostedRange.EndDateTimeOffset);
        command.AddParameter("@LicenseIds", licenseIds);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var threads = new List<Thread>();
            while (await reader.ReadAsync()) threads.Add(ForumRunDataReader.ReadThread(reader, true));

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                var thread = threads.FirstOrDefault(x => x.Id == reader.ReadGuid(0));
                var tagId = reader.ReadGuid(1);
                if (thread is not null && tagId.HasValue && thread.Tags.All(x => x.Id != tagId))
                    thread.Tags.Add(ForumRunDataReader.ReadTag(reader, 1));
            }

            return threads;
        });
    }

    public static async Task<Thread?> Execute(String connection, Guid? sessionId, Guid? id, IEnumerable<Guid?> licenseIds)
    {
        await using var command = new SqlCommand { CommandText = "ContentDisplay.ForumRunThreadSelect" };
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", id);
        command.AddParameter("@LicenseIds", licenseIds);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync()) return null;

            var thread = ForumRunDataReader.ReadThread(reader, false);

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                thread.Comment.CommentMedias.Add(ForumRunDataReader.ReadCommentMedia(reader));

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
                thread.Comment.Reactions.Add(ForumRunDataReader.ReadReaction(reader));

            await reader.NextResultAsync();
            while (await reader.ReadAsync())
            {
                var bundleId = reader.ReadGuid(0);
                var bundle = thread.ForumBundles.FirstOrDefault(x => x.BundleId == bundleId);
                if (bundle is null)
                {
                    bundle = ForumRunDataReader.ReadForumBundle(reader);
                    thread.ForumBundles.Add(bundle);
                }

                var tagId = reader.ReadGuid(4);
                if (!tagId.HasValue) continue;

                if (bundle.Tags.All(x => x.Id != tagId))
                    bundle.Tags.Add(new()
                {
                    Id = tagId,
                    Name = reader.ReadString(5),
                });

                if (bundle.ThreadRule != ForumBundle.Rules.NotUsed
                    && reader.ReadBoolean(6) == true
                    && thread.Tags.All(x => x.Id != tagId))
                    thread.Tags.Add(ForumRunDataReader.ReadTag(reader, 4));
            }

            return thread;
        });
    }
}