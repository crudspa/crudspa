namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolContactSchoolYearUpdateSelectionsBySchoolContact
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolContactSchoolYearUpdateSelectionsBySchoolContact";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SchoolContactId", schoolContact.Id);
        command.AddParameter("@SchoolYears", schoolContact.SchoolYears);

        await command.Execute(connection, transaction);
    }
}