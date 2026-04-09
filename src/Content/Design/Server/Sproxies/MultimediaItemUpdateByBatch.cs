namespace Crudspa.Content.Design.Server.Sproxies;

public static class MultimediaItemUpdateByBatch
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MultimediaItem multimediaItem)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MultimediaItemUpdateByBatch";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", multimediaItem.Id);
        command.AddParameter("@MultimediaElementId", multimediaItem.MultimediaElementId);
        command.AddParameter("@BoxId", multimediaItem.Box.Id);
        command.AddParameter("@ItemId", multimediaItem.Item.Id);
        command.AddParameter("@MediaTypeIndex", multimediaItem.MediaTypeIndex);
        command.AddParameter("@AudioId", multimediaItem.AudioFile.Id);
        command.AddParameter("@ButtonId", multimediaItem.Button.Id);
        command.AddParameter("@ImageId", multimediaItem.ImageFile.Id);
        command.AddParameter("@Text", multimediaItem.Text);
        command.AddParameter("@VideoId", multimediaItem.VideoFile.Id);
        command.AddParameter("@Ordinal", multimediaItem.Ordinal);

        await command.Execute(connection, transaction);
    }
}