using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;

namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ForumInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Forum forum)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ForumInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 75, forum.Name);
        command.AddParameter("@Description", forum.Description);
        command.AddParameter("@BodyTemplateId", forum.BodyTemplateId);
        command.AddParameter("@Pinned", forum.Pinned);
        command.AddParameter("@DistrictId", forum.DistrictId);
        command.AddParameter("@SchoolId", forum.SchoolId);
        command.AddParameter("@InnovatorsOnly", forum.InnovatorsOnly ?? false);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}