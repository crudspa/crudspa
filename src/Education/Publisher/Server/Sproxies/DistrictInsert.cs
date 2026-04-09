namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, District district)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@StudentIdNumberLabel", 50, district.StudentIdNumberLabel);
        command.AddParameter("@AssessmentExplainer", district.AssessmentExplainer);
        command.AddParameter("@OrganizationId", district.OrganizationId);
        command.AddParameter("@AddressId", district.AddressId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}