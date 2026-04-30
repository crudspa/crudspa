namespace Crudspa.Content.Design.Server.Sproxies;

public static class OptionsAnswerChoiceSelectForOptionsAnswer
{
    public static async Task<IList<OptionsAnswerChoice>> Execute(String connection, Guid? optionsAnswerId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.OptionsAnswerChoiceSelectForOptionsAnswer";

        command.AddParameter("@OptionsAnswerId", optionsAnswerId);

        return await command.ReadAll<OptionsAnswerChoice>(connection, reader => new()
        {
            Id = reader.ReadGuid(0),
            OptionsAnswerId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            Ordinal = reader.ReadInt32(3),
        });
    }
}