using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;

namespace Crudspa.Education.School.Server.Sproxies;

public static class PostSelectTreeForParent
{
    public static async Task<IList<Post>> Execute(String connection, Guid? sessionId, Guid? parentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.PostSelectTreeForParent";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ParentId", parentId);

        var allPosts = await command.ReadAll(connection, ReadPost);

        var posts = allPosts.Where(x => x.ParentId.Equals(parentId)).OrderBy(x => x.Posted).ToList();

        foreach (var post in posts)
            AddChildren(post, allPosts);

        return posts;
    }

    private static void AddChildren(Post post, IList<Post> allPosts)
    {
        post.Children = allPosts.Where(x => x.ParentId.Equals(post.Id)).OrderBy(x => x.Posted).ToObservable();

        foreach (var childPost in post.Children)
            AddChildren(childPost, allPosts);
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ForumId = reader.ReadGuid(1),
            ParentId = reader.ReadGuid(2),
            Pinned = reader.ReadBoolean(3),
            Body = reader.ReadString(4),
            AudioFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(9),
                OptimizedBlobId = reader.ReadGuid(10),
                OptimizedFormat = reader.ReadString(11),
            },
            ImageFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                Width = reader.ReadInt32(16),
                Height = reader.ReadInt32(17),
                Caption = reader.ReadString(18),
            },
            PdfFile = new()
            {
                Id = reader.ReadGuid(19),
                BlobId = reader.ReadGuid(20),
                Name = reader.ReadString(21),
                Format = reader.ReadString(22),
                Description = reader.ReadString(23),
            },
            VideoFile = new()
            {
                Id = reader.ReadGuid(24),
                BlobId = reader.ReadGuid(25),
                Name = reader.ReadString(26),
                Format = reader.ReadString(27),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(28),
                OptimizedBlobId = reader.ReadGuid(29),
                OptimizedFormat = reader.ReadString(30),
            },
            ById = reader.ReadGuid(31),
            ByOrganizationName = reader.ReadString(32),
            Posted = reader.ReadDateTimeOffset(33),
            Edited = reader.ReadDateTimeOffset(34),
            ByFirstName = reader.ReadString(35),
            ByLastName = reader.ReadString(36),
        };
    }
}