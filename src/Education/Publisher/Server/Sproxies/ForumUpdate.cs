using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", forum.Id);
        command.AddParameter("@Name", 75, forum.Name);
        command.AddParameter("@Description", forum.Description);
        command.AddParameter("@BodyTemplateId", forum.BodyTemplateId);
        command.AddParameter("@Pinned", forum.Pinned);
        command.AddParameter("@DistrictId", forum.DistrictId);
        command.AddParameter("@SchoolId", forum.SchoolId);
        command.AddParameter("@InnovatorsOnly", forum.InnovatorsOnly ?? false);

        await command.Execute(connection, transaction);
    }
}