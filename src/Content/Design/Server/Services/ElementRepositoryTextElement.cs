namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryTextElement(
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var textElementId = Guid.NewGuid();
        var elementId = Guid.NewGuid();

        var sectionElement = new SectionElement
        {
            Element = new()
            {
                Id = elementId,
                SectionId = sectionId,
                TypeId = elementType.Id,
                ElementType = elementType,
                Ordinal = ordinal,
                Item = new()
                {
                    BasisId = BasisIds.Auto,
                    Grow = "0",
                    Shrink = "1",
                    AlignSelfId = AlignSelfIds.Auto,
                },
            },
        };

        sectionElement.SetConfig(new TextElement
        {
            Id = textElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var textElement = element.RequireConfig<TextElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(textElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var textElement = element.RequireConfig<TextElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        textElement.Text = htmlSanitizer.Sanitize(textElement.Text);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        textElement.ElementId = elementId;

        await TextElementInsert.Execute(connection, transaction, sessionId, textElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var textElement = element.RequireConfig<TextElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        textElement.Text = htmlSanitizer.Sanitize(textElement.Text);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);
        await TextElementUpdate.Execute(connection, transaction, sessionId, textElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var textElement = element.RequireConfig<TextElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await TextElementDelete.Execute(connection, transaction, sessionId, textElement);
    }
}