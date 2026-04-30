namespace Crudspa.Content.Design.Server.Sproxies;

public static class OptionsAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, OptionsAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.OptionsAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@Orientation", answer.Orientation);
        command.AddParameter("@AllowOther", answer.AllowOther);
        command.AddParameter("@OtherLabel", 50, answer.OtherLabel);
        command.AddParameter("@MinSelections", answer.MinSelections);
        command.AddParameter("@MaxSelections", answer.MaxSelections);
        command.AddParameter("@Ordering", answer.Ordering);

        await command.Execute(connection, transaction);
    }
}