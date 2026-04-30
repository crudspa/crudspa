namespace Crudspa.Content.Design.Server.Sproxies;

public static class AnswerTypeSelectFull
{
    public static async Task<IList<AnswerType>> Execute(String connection)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.AnswerTypeSelectFull";

        return await command.ReadAll<AnswerType>(connection, reader => new()
        {
            Id = reader.ReadGuid(0),
            Name = reader.ReadString(1),
            DesignView = reader.ReadString(2),
            DisplayView = reader.ReadString(3),
        });
    }
}