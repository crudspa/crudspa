namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class UnitLicenseUpdateRelations
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, UnitLicense unitLicense)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.UnitLicenseUpdateRelations";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", unitLicense.Id);
        command.AddParameter("@AllBooks", unitLicense.AllBooks ?? true);
        command.AddParameter("@AllLessons", unitLicense.AllLessons ?? true);
        command.AddParameter("@Books", unitLicense.Books);
        command.AddParameter("@Lessons", unitLicense.Lessons);

        await command.Execute(connection, transaction);
    }
}