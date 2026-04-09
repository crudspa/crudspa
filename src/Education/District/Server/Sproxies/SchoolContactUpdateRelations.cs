namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolContactUpdateRelations
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolContact schoolContact)
    {
        var schoolYears = schoolContact.Relations.First(x => x.Name.IsBasically(SchoolContactSelectRelations.SchoolYears)).Selections;

        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolContactUpdateRelations";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SchoolContactId", schoolContact.Id);
        command.AddParameter("@SchoolYears", schoolYears);

        await command.Execute(connection, transaction);
    }
}