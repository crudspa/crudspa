namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class DistrictUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, District district)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.DistrictUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", district.Id);
        command.AddParameter("@StudentIdNumberLabel", 50, district.StudentIdNumberLabel);
        command.AddParameter("@AssessmentExplainer", district.AssessmentExplainer);
        command.AddParameter("@AddressId", district.AddressId);

        await command.Execute(connection, transaction);
    }
}