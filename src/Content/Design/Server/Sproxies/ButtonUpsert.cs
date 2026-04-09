namespace Crudspa.Content.Design.Server.Sproxies;

public static class ButtonUpsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Button button)
    {
        if (button.Id.HasNothing())
            button.Id = Guid.NewGuid();

        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.ButtonUpsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", button.Id);
        command.AddParameter("@Internal", button.Internal ?? true);
        command.AddParameter("@Path", 250, button.Path);
        command.AddParameter("@Text", 250, button.Text);
        command.AddParameter("@ShapeIndex", button.ShapeIndex);
        command.AddParameter("@GraphicIndex", button.GraphicIndex);
        command.AddParameter("@TextTypeIndex", button.TextTypeIndex);
        command.AddParameter("@OrientationIndex", button.OrientationIndex);
        command.AddParameter("@IconId", button.IconId);
        command.AddParameter("@ImageId", button.ImageFile.Id);
        command.AddParameter("@BoxId", button.Box.Id);

        await command.Execute(connection, transaction);

        return button.Id;
    }
}