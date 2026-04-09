namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Assessment assessment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessment.Id);

        await command.Execute(connection, transaction);
    }
}