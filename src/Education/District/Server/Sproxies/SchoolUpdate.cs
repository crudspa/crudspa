namespace Crudspa.Education.District.Server.Sproxies;

public static class SchoolUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationDistrict.SchoolUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", school.Id);
        command.AddParameter("@Key", 100, school.Key);
        command.AddParameter("@CommunityId", school.CommunityId);
        command.AddParameter("@Treatment", school.Treatment ?? false);
        command.AddParameter("@AddressId", school.AddressId);

        await command.Execute(connection, transaction);
    }
}