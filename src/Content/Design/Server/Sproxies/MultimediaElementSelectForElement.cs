using Crudspa.Content.Display.Server;

namespace Crudspa.Content.Design.Server.Sproxies;

public static class MultimediaElementSelectForElement
{
    public static async Task<MultimediaElement?> Execute(String connection, Guid? elementId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MultimediaElementSelectForElement";

        command.AddParameter("@ElementId", elementId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var multimediaElement = PageDataReaders.ReadMultimediaElement(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                multimediaElement.MultimediaItems.Add(PageDataReaders.ReadMultimediaItem(reader));

            return multimediaElement;
        });
    }
}