namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Assessment assessment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", assessment.Id);
        command.AddParameter("@Name", 100, assessment.Name);
        command.AddParameter("@StatusId", assessment.StatusId);
        command.AddParameter("@GradeId", assessment.GradeId);
        command.AddParameter("@AvailableStart", assessment.AvailableStart);
        command.AddParameter("@AvailableEnd", assessment.AvailableEnd);
        command.AddParameter("@CategoryId", assessment.CategoryId);
        command.AddParameter("@ImageFileId", assessment.ImageFileFile.Id);

        await command.Execute(connection, transaction);
    }
}