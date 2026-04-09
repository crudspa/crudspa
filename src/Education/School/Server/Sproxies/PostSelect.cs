using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;

namespace Crudspa.Education.School.Server.Sproxies;

public static class PostSelect
{
    public static async Task<Post?> Execute(String connection, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.PostSelect";

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
                Id = reader.ReadGuid(20),
                BlobId = reader.ReadGuid(21),
                Name = reader.ReadString(22),
                Format = reader.ReadString(23),
                Description = reader.ReadString(24),
            },
            VideoFile = new()
            {
                Id = reader.ReadGuid(25),
                BlobId = reader.ReadGuid(26),
                Name = reader.ReadString(27),
                Format = reader.ReadString(28),
                OptimizedStatus = reader.ReadEnum<VideoFile.OptimizationStatus>(29),
                OptimizedBlobId = reader.ReadGuid(30),
                OptimizedFormat = reader.ReadString(31),
            },
            ById = reader.ReadGuid(32),
            ByOrganizationName = reader.ReadString(33),
            Posted = reader.ReadDateTimeOffset(34),
            Edited = reader.ReadDateTimeOffset(35),
            Type = reader.ReadEnum<Post.PostTypes>(36),
            GradeId = reader.ReadGuid(37),
            SubjectId = reader.ReadGuid(38),
            ByFirstName = reader.ReadString(39),
            ByLastName = reader.ReadString(40),
            GradeName = reader.ReadString(41),
            SubjectName = reader.ReadString(42),
            ReactionCharacter = reader.ReadString(43),
        };
    }
}