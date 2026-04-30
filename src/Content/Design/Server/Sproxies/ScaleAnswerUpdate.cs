namespace Crudspa.Content.Design.Server.Sproxies;

public static class ScaleAnswerUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ScaleAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ScaleAnswerUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", answer.Id);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@RatingKind", answer.RatingKind);
        command.AddParameter("@LikertKind", answer.LikertKind);
        command.AddParameter("@RatingMin", answer.RatingMin);
        command.AddParameter("@RatingMax", answer.RatingMax);
        command.AddParameter("@Ordering", answer.Ordering);

        await command.Execute(connection, transaction);
    }
}