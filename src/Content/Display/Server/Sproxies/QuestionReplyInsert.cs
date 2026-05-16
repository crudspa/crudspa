namespace Crudspa.Content.Display.Server.Sproxies;

public static class QuestionReplyInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, QuestionReply reply)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.QuestionReplyInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ElementId", reply.ElementId);
        command.AddParameter("@SurveyReplyId", reply.SurveyReplyId);
        command.AddParameter("@QuestionId", reply.QuestionId);
        command.AddParameter("@Submitted", reply.Submitted);
        command.AddParameter("@BoolValue", reply.BoolValue);
        command.AddParameter("@TextValue", reply.TextValue);
        command.AddParameter("@HtmlValue", reply.HtmlValue);
        command.AddParameter("@DateValue", reply.DateValue);
        command.AddParameter("@TimeValue", reply.TimeValue?.ToTimeSpan());
        command.AddParameter("@DateTimeValue", reply.DateTimeValue);
        command.AddParameter("@IntegerValue", reply.IntegerValue);
        command.AddParameter("@DecimalValue", reply.DecimalValue);
        command.AddParameter("@CurrencyValue", reply.CurrencyValue);
        command.AddParameter("@OtherBoolValue", reply.OtherBoolValue);
        command.AddParameter("@OtherTextValue", 150, reply.OtherTextValue);
        command.AddParameter("@AudioId", reply.AudioId);
        command.AddParameter("@ImageId", reply.ImageId);
        command.AddParameter("@PdfId", reply.PdfId);
        command.AddParameter("@VideoId", reply.VideoId);
        command.AddParameter("@PostalId", reply.PostalId);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}