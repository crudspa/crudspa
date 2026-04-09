using Post = Crudspa.Education.School.Shared.Contracts.Data.Post;

namespace Crudspa.Education.School.Server.Sproxies;

public static class PostInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.PostInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ForumId", post.ForumId);
        command.AddParameter("@ParentId", post.ParentId);
        command.AddParameter("@Pinned", post.Pinned ?? false);
        command.AddParameter("@Body", post.Body);
        command.AddParameter("@AudioId", post.AudioFile.Id);
        command.AddParameter("@ImageId", post.ImageFile.Id);
        command.AddParameter("@PdfId", post.PdfFile.Id);
        command.AddParameter("@VideoId", post.VideoFile.Id);
        command.AddParameter("@Type", post.Type);
        command.AddParameter("@GradeId", post.GradeId);
        command.AddParameter("@SubjectId", post.SubjectId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}