namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryButton(IServerConfigService configService, IFileService fileService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var buttonElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new ButtonElement
        {
            Id = buttonElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var buttonElement = element.RequireConfig<ButtonElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(buttonElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var buttonElement = element.RequireConfig<ButtonElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        buttonElement.ElementId = elementId;

        var buttonBackgroundImageResponse = await fileService.SaveImage(new(sessionId, buttonElement.Button.Box.BackgroundImageFile));
        if (!buttonBackgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + buttonBackgroundImageResponse.ErrorMessages);
        buttonElement.Button.Box.BackgroundImageFile.Id = buttonBackgroundImageResponse.Value!.Id;
        buttonElement.Button.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, buttonElement.Button.Box);

        var buttonImageResponse = await fileService.SaveImage(new(sessionId, ResolveButtonImage(buttonElement.Button)));
        if (!buttonImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + buttonImageResponse.ErrorMessages);
        buttonElement.Button.ImageFile.Id = buttonImageResponse.Value!.Id;

        buttonElement.Button.Id = await ButtonUpsert.Execute(connection, transaction, sessionId, buttonElement.Button);

        await ButtonElementInsert.Execute(connection, transaction, sessionId, buttonElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var buttonElement = element.RequireConfig<ButtonElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var existingButtonElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, buttonElement.Button.Box);
        var existingButton = await ButtonSelect.Execute(configService.Fetch().Database, sessionId, buttonElement.Button.Id);
        var buttonBackgroundImageResponse = await fileService.SaveImage(new(sessionId, buttonElement.Button.Box.BackgroundImageFile), existingButtonElementBox?.BackgroundImageFile);
        if (!buttonBackgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + buttonBackgroundImageResponse.ErrorMessages);
        buttonElement.Button.Box.BackgroundImageFile.Id = buttonBackgroundImageResponse.Value!.Id;
        buttonElement.Button.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, buttonElement.Button.Box);

        var buttonImageResponse = await fileService.SaveImage(new(sessionId, ResolveButtonImage(buttonElement.Button)), existingButton?.ImageFile);
        if (!buttonImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + buttonImageResponse.ErrorMessages);
        buttonElement.Button.ImageFile.Id = buttonImageResponse.Value!.Id;

        buttonElement.Button.Id = await ButtonUpsert.Execute(connection, transaction, sessionId, buttonElement.Button);

        await ButtonElementUpdate.Execute(connection, transaction, sessionId, buttonElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var buttonElement = element.RequireConfig<ButtonElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await ButtonElementDelete.Execute(connection, transaction, sessionId, buttonElement);
    }

    private static ImageFile ResolveButtonImage(Button button) =>
        button.GraphicIndex == Button.Graphics.Image ? button.ImageFile : new();
}