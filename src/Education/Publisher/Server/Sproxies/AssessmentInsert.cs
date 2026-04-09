namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class AssessmentInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Assessment assessment)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.AssessmentInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Name", 100, assessment.Name);
        command.AddParameter("@StatusId", assessment.StatusId);
        command.AddParameter("@GradeId", assessment.GradeId);
        command.AddParameter("@AvailableStart", assessment.AvailableStart);
        command.AddParameter("@AvailableEnd", assessment.AvailableEnd);
        command.AddParameter("@CategoryId", assessment.CategoryId);
        command.AddParameter("@ImageFileId", assessment.ImageFileFile.Id);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}