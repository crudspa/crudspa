using Post = Crudspa.Education.Publisher.Shared.Contracts.Data.Post;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class PostUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Post post)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.PostUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", post.Id);
        command.AddParameter("@Pinned", post.Pinned ?? false);
        command.AddParameter("@Body", post.Body);
        command.AddParameter("@AudioId", post.AudioFile.Id);
        command.AddParameter("@ImageId", post.ImageFile.Id);
        command.AddParameter("@PdfId", post.PdfFile.Id);
        command.AddParameter("@VideoId", post.VideoFile.Id);
        command.AddParameter("@Type", post.Type);
        command.AddParameter("@GradeId", post.GradeId);
        command.AddParameter("@SubjectId", post.SubjectId);

        await command.Execute(connection, transaction);
    }
}