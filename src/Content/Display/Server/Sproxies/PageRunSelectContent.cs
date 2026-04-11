namespace Crudspa.Content.Display.Server.Sproxies;

public static class PageRunSelectContent
{
    public static async Task<Page?> Execute(String connection, Page page, Guid? sessionId, Guid? sectionId = null)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDisplay.PageRunSelectContent";

        command.AddParameter("@Id", page.Id);
        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@SectionId", sectionId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            page = PageDataReaders.ReadPage(reader);

            await reader.NextResultAsync();
            page.Sections = (await PageDataReaders.ReadSectionsWithElements(reader)).ToObservable();

            return page;
        });
    }
}