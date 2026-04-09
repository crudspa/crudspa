namespace Crudspa.Education.School.Server.Sproxies;

public static class SchoolContactInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.SchoolContactInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@TitleId", schoolContact.TitleId);
        command.AddParameter("@TestAccount", schoolContact.TestAccount ?? false);
        command.AddParameter("@ContactId", schoolContact.ContactId);
        command.AddParameter("@UserId", schoolContact.UserId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}