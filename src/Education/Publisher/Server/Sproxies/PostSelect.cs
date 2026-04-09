using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PostSelect
{
    public static async Task<Post?> Execute(String connection, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PostSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);

        return await command.ReadSingle(connection, ReadPost);
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
            Type = reader.ReadEnum<Post.PostTypes>(35),
            GradeId = reader.ReadGuid(36),
            SubjectId = reader.ReadGuid(37),
            ByFirstName = reader.ReadString(38),
            ByLastName = reader.ReadString(39),
            GradeName = reader.ReadString(40),
            SubjectName = reader.ReadString(41),
            ReactionCharacter = reader.ReadString(42),
        };
    }
}