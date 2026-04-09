namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolContactDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SchoolContact schoolContact)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolContactDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", schoolContact.Id);

        await command.Execute(connection, transaction);
    }
}