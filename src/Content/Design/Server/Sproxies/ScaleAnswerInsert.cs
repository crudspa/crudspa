namespace Crudspa.Content.Design.Server.Sproxies;

public static class ScaleAnswerInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ScaleAnswer answer)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ScaleAnswerInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@QuestionId", answer.QuestionId);
        command.AddParameter("@Kind", answer.Kind);
        command.AddParameter("@RatingKind", answer.RatingKind);
        command.AddParameter("@LikertKind", answer.LikertKind);
        command.AddParameter("@RatingMin", answer.RatingMin);
        command.AddParameter("@RatingMax", answer.RatingMax);
        command.AddParameter("@Ordering", answer.Ordering);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}