using IHtmlSanitizer = Crudspa.Framework.Core.Server.Contracts.Behavior.IHtmlSanitizer;

namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryMultimedia(IServerConfigService configService, IFileService fileService, IHtmlSanitizer htmlSanitizer) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var multimediaElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new MultimediaElement
        {
            Id = multimediaElementId,
            ElementId = elementId,
            Container = new()
            {
                DirectionId = DirectionIds.Row,
                WrapId = WrapIds.Wrap,
                JustifyContentId = JustifyContentIds.Center,
                AlignItemsId = AlignItemsIds.Center,
                AlignContentId = AlignContentIds.Start,
            },
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var multimediaElement = element.RequireConfig<MultimediaElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(multimediaElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var multimediaElement = element.RequireConfig<MultimediaElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        multimediaElement.ElementId = elementId;
        multimediaElement.Container.Id = await ContainerUpsert.Execute(connection, transaction, sessionId, multimediaElement.Container);
        multimediaElement.Id = await MultimediaElementInsert.Execute(connection, transaction, sessionId, multimediaElement);

        foreach (var multimediaItem in multimediaElement.MultimediaItems)
        {
            multimediaItem.MultimediaElementId = multimediaElement.Id;
            multimediaItem.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, multimediaItem.Box);
            multimediaItem.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, multimediaItem.Item);

            switch (multimediaItem.MediaTypeIndex)
            {
                case MultimediaItem.MediaTypes.Audio:
                    var audioResponse = await fileService.SaveAudio(new(sessionId, multimediaItem.AudioFile));
                    if (!audioResponse.Ok)
                        throw new("Call to IFileService.SaveAudio() failed. " + audioResponse.ErrorMessages);
                    if (!audioResponse.Value.Id.HasValue)
                        throw new("Audio file is required.");
                    multimediaItem.AudioFile.Id = audioResponse.Value.Id;
                    break;
                case MultimediaItem.MediaTypes.Button:
                    var buttonImageResponse = await fileService.SaveImage(new(sessionId, ResolveButtonImage(multimediaItem.Button)));
                    if (!buttonImageResponse.Ok)
                        throw new("Call to IFileService.SaveImage() failed. " + buttonImageResponse.ErrorMessages);
                    multimediaItem.Button.ImageFile.Id = buttonImageResponse.Value!.Id;
                    multimediaItem.Button.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, multimediaItem.Button.Box);
                    multimediaItem.Button.Id = await ButtonUpsert.Execute(connection, transaction, sessionId, multimediaItem.Button);
                    break;
                case MultimediaItem.MediaTypes.Image:
                    var imageResponse = await fileService.SaveImage(new(sessionId, multimediaItem.ImageFile));
                    if (!imageResponse.Ok)
                        throw new("Call to IFileService.SaveImage() failed. " + imageResponse.ErrorMessages);
                    if (!imageResponse.Value.Id.HasValue)
                        throw new("Image file is required.");
                    multimediaItem.ImageFile.Id = imageResponse.Value.Id;
                    break;
                case MultimediaItem.MediaTypes.Text:
                    multimediaItem.Text = htmlSanitizer.Sanitize(multimediaItem.Text);
                    break;
                case MultimediaItem.MediaTypes.Video:
                    var videoResponse = await fileService.SaveVideo(new(sessionId, multimediaItem.VideoFile));
                    if (!videoResponse.Ok)
                        throw new("Call to IFileService.SaveVideo() failed. " + videoResponse.ErrorMessages);
                    if (!videoResponse.Value.Id.HasValue)
                        throw new("Video file is required.");
                    multimediaItem.VideoFile.Id = videoResponse.Value.Id;
                    break;
                default:
                    throw new($"MediaTypeIndex {multimediaItem.MediaTypeIndex} not yet supported.");
            }

            await MultimediaItemInsertByBatch.Execute(connection, transaction, sessionId, multimediaItem);
        }

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var multimediaElement = element.RequireConfig<MultimediaElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageUpdateResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageUpdateResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageUpdateResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageUpdateResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var existingMultimediaElement = await MultimediaElementSelectForElement.Execute(configService.Fetch().Database, element.Id);

        multimediaElement.Container.Id = await ContainerUpsert.Execute(connection, transaction, sessionId, multimediaElement.Container);

        foreach (var multimediaItem in multimediaElement.MultimediaItems)
        {
            multimediaItem.MultimediaElementId = multimediaElement.Id;
            multimediaItem.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, multimediaItem.Box);
            multimediaItem.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, multimediaItem.Item);

            var existingMultimediaItem = existingMultimediaElement?.MultimediaItems.FirstOrDefault(x => x.Id.Equals(multimediaItem.Id));

            switch (multimediaItem.MediaTypeIndex)
            {
                case MultimediaItem.MediaTypes.Audio:
                    var audioResponse = await fileService.SaveAudio(new(sessionId, multimediaItem.AudioFile), existingMultimediaItem?.AudioFile);
                    if (!audioResponse.Ok)
                        throw new("Call to IFileService.SaveAudio() failed. " + audioResponse.ErrorMessages);
                    if (!audioResponse.Value.Id.HasValue)
                        throw new("Audio file is required.");
                    multimediaItem.AudioFile.Id = audioResponse.Value.Id;
                    break;
                case MultimediaItem.MediaTypes.Button:
                    var buttonImageResponse = await fileService.SaveImage(new(sessionId, ResolveButtonImage(multimediaItem.Button)), existingMultimediaItem?.Button.ImageFile);
                    if (!buttonImageResponse.Ok)
                        throw new("Call to IFileService.SaveImage() failed. " + buttonImageResponse.ErrorMessages);
                    multimediaItem.Button.ImageFile.Id = buttonImageResponse.Value!.Id;
                    multimediaItem.Button.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, multimediaItem.Button.Box);
                    multimediaItem.Button.Id = await ButtonUpsert.Execute(connection, transaction, sessionId, multimediaItem.Button);
                    break;
                case MultimediaItem.MediaTypes.Image:
                    var imageResponse = await fileService.SaveImage(new(sessionId, multimediaItem.ImageFile), existingMultimediaItem?.ImageFile);
                    if (!imageResponse.Ok)
                        throw new("Call to IFileService.SaveImage() failed. " + imageResponse.ErrorMessages);
                    if (!imageResponse.Value.Id.HasValue)
                        throw new("Image file is required.");
                    multimediaItem.ImageFile.Id = imageResponse.Value.Id;
                    break;
                case MultimediaItem.MediaTypes.Text:
                    multimediaItem.Text = htmlSanitizer.Sanitize(multimediaItem.Text);
                    break;
                case MultimediaItem.MediaTypes.Video:
                    var videoResponse = await fileService.SaveVideo(new(sessionId, multimediaItem.VideoFile), existingMultimediaItem?.VideoFile);
                    if (!videoResponse.Ok)
                        throw new("Call to IFileService.SaveVideo() failed. " + videoResponse.ErrorMessages);
                    if (!videoResponse.Value.Id.HasValue)
                        throw new("Video file is required.");
                    multimediaItem.VideoFile.Id = videoResponse.Value.Id;
                    break;
                default:
                    throw new($"MediaTypeIndex {multimediaItem.MediaTypeIndex} not yet supported.");
            }
        }

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existingMultimediaElement!.MultimediaItems,
            multimediaElement.MultimediaItems,
            MultimediaItemInsertByBatch.Execute,
            MultimediaItemUpdateByBatch.Execute,
            MultimediaItemDeleteByBatch.Execute);

        multimediaElement.MultimediaItems.EnsureOrder();
        await MultimediaItemUpdateOrdinalsByBatch.Execute(connection, transaction, sessionId, multimediaElement.MultimediaItems);

        await MultimediaElementUpdate.Execute(connection, transaction, sessionId, multimediaElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var multimediaElement = element.RequireConfig<MultimediaElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await MultimediaElementDelete.Execute(connection, transaction, sessionId, multimediaElement);
    }

    private static ImageFile ResolveButtonImage(Button button) =>
        button.GraphicIndex == Button.Graphics.Image ? button.ImageFile : new();
}