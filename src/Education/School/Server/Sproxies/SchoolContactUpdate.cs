namespace Crudspa.Education.School.Server.Sproxies;

public static class SchoolContactUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolContactUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", schoolContact.Id);
        command.AddParameter("@TitleId", schoolContact.TitleId);
        command.AddParameter("@TestAccount", schoolContact.TestAccount ?? false);
        command.AddParameter("@UserId", schoolContact.UserId);

        await command.Execute(connection, transaction);
    }
}