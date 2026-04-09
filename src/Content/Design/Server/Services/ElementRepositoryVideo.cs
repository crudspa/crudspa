namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryVideo(IServerConfigService configService, IFileService fileService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var videoElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new VideoElement
        {
            Id = videoElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var videoElement = element.RequireConfig<VideoElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(videoElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var videoElement = element.RequireConfig<VideoElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        videoElement.ElementId = elementId;

        var videoResponse = await fileService.SaveVideo(new(sessionId, videoElement.FileFile));
        if (!videoResponse.Ok)
            throw new("Call to IFileService.SaveVideo() failed. " + videoResponse.ErrorMessages);
        if (!videoResponse.Value.Id.HasValue)
            throw new("Video file is required.");

        videoElement.FileFile.Id = videoResponse.Value.Id;

        var imageResponse = await fileService.SaveImage(new(sessionId, videoElement.Poster));
        if (!imageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + imageResponse.ErrorMessages);

        videoElement.Poster.Id = imageResponse.Value.Id;

        await VideoInsert.Execute(connection, transaction, sessionId, videoElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var videoElement = element.RequireConfig<VideoElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var videoResponse = await fileService.SaveVideo(new(sessionId, videoElement.FileFile), videoElement.FileFile.Id);
        if (!videoResponse.Ok)
            throw new("Call to IFileService.SaveVideo() failed. " + videoResponse.ErrorMessages);
        if (!videoResponse.Value.Id.HasValue)
            throw new("Video file is required.");

        videoElement.FileFile.Id = videoResponse.Value.Id;

        var imageResponse = await fileService.SaveImage(new(sessionId, videoElement.Poster), videoElement.Poster.Id);
        if (!imageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + imageResponse.ErrorMessages);

        videoElement.Poster.Id = imageResponse.Value.Id;

        await VideoUpdate.Execute(connection, transaction, sessionId, videoElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var videoElement = element.RequireConfig<VideoElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await VideoDelete.Execute(connection, transaction, sessionId, videoElement);
    }
}