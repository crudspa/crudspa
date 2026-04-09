using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;
using PostSearch = Crudspa.Education.School.Shared.Contracts.Data.PostSearch;

namespace Crudspa.Education.School.Server.Sproxies;

public static class PostSelectWhereForForum
{
    public static async Task<IList<Post>> Execute(String connection, PostSearch search)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.PostSelectWhereForForum";

        command.AddParameter("@ForumId", search.ParentId);
        command.AddParameter("@PageNumber", search.Paged.PageNumber);
        command.AddParameter("@PageSize", search.Paged.PageSize);
        command.AddParameter("@SearchText", 50, search.Text);
        command.AddParameter("@SortField", search.Sort.Field);
        command.AddParameter("@SortAscending", search.Sort.Ascending);
        command.AddParameter("@Grades", search.Grades);
        command.AddParameter("@Subjects", search.Subjects);

        return await command.ReadAll(connection, ReadPost);
    }

    private static Post ReadPost(SqlDataReader reader)
    {
        return new()
        {
            TotalCount = reader.ReadInt32(1),
            Id = reader.ReadGuid(2),
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
            CommentCount = reader.ReadInt32(42),
        };
    }
}