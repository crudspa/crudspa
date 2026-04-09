namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class SchoolInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, School school)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.SchoolInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@DistrictId", school.DistrictId);
        command.AddParameter("@Key", 100, school.Key);
        command.AddParameter("@CommunityId", school.CommunityId);
        command.AddParameter("@Treatment", school.Treatment ?? false);
        command.AddParameter("@OrganizationId", school.OrganizationId);
        command.AddParameter("@AddressId", school.AddressId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}