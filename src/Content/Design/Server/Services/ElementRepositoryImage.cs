namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryImage(IServerConfigService configService, IFileService fileService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var imageElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new ImageElement
        {
            Id = imageElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var imageElement = element.RequireConfig<ImageElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(imageElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var imageElement = element.RequireConfig<ImageElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        imageElement.ElementId = elementId;

        var imageResponse = await fileService.SaveImage(new(sessionId, imageElement.FileFile));
        if (!imageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + imageResponse.ErrorMessages);
        if (!imageResponse.Value.Id.HasValue)
            throw new("Image file is required.");

        imageElement.FileFile.Id = imageResponse.Value.Id;

        await ImageInsert.Execute(connection, transaction, sessionId, imageElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var imageElement = element.RequireConfig<ImageElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var imageResponse = await fileService.SaveImage(new(sessionId, imageElement.FileFile), imageElement.FileFile.Id);
        if (!imageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + imageResponse.ErrorMessages);
        if (!imageResponse.Value.Id.HasValue)
            throw new("Image file is required.");

        imageElement.FileFile.Id = imageResponse.Value!.Id;

        await ImageUpdate.Execute(connection, transaction, sessionId, imageElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var imageElement = element.RequireConfig<ImageElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await ImageDelete.Execute(connection, transaction, sessionId, imageElement);
    }
}