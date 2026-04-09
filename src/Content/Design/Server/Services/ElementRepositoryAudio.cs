namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryAudio(IServerConfigService configService, IFileService fileService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var audioElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new AudioElement
        {
            Id = audioElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var audioElement = element.RequireConfig<AudioElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(audioElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var audioElement = element.RequireConfig<AudioElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        audioElement.ElementId = elementId;

        var audioResponse = await fileService.SaveAudio(new(sessionId, audioElement.FileFile));
        if (!audioResponse.Ok)
            throw new("Call to IFileService.SaveAudio() failed. " + audioResponse.ErrorMessages);
        if (!audioResponse.Value.Id.HasValue)
            throw new("Audio file is required.");

        audioElement.FileFile.Id = audioResponse.Value.Id;

        await AudioInsert.Execute(connection, transaction, sessionId, audioElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var audioElement = element.RequireConfig<AudioElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var audioResponse = await fileService.SaveAudio(new(sessionId, audioElement.FileFile), audioElement.FileFile.Id);
        if (!audioResponse.Ok)
            throw new("Call to IFileService.SaveAudio() failed. " + audioResponse.ErrorMessages);
        if (!audioResponse.Value.Id.HasValue)
            throw new("Audio file is required.");

        audioElement.FileFile.Id = audioResponse.Value.Id;

        await AudioUpdate.Execute(connection, transaction, sessionId, audioElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var audioElement = element.RequireConfig<AudioElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await AudioDelete.Execute(connection, transaction, sessionId, audioElement);
    }
}