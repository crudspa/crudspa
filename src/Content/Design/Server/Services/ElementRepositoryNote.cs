namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryNote(IServerConfigService configService, IFileService fileService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var noteElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new NoteElement
        {
            Id = noteElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var noteElement = element.RequireConfig<NoteElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(noteElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var noteElement = element.RequireConfig<NoteElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        noteElement.ElementId = elementId;

        var noteImageFileResponse = await fileService.SaveImage(new(sessionId, noteElement.ImageFileFile));
        if (!noteImageFileResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + noteImageFileResponse.ErrorMessages);
        if (!noteImageFileResponse.Value!.Id.HasValue)
            throw new("Image file is required.");
        noteElement.ImageFileFile.Id = noteImageFileResponse.Value.Id;

        noteElement.Id = await NoteInsert.Execute(connection, transaction, sessionId, noteElement);

        noteElement.NoteImages.EnsureOrder();

        foreach (var noteImage in noteElement.NoteImages)
        {
            noteImage.NoteId = noteElement.Id;
            var noteImageResponse = await fileService.SaveImage(new(sessionId, noteImage.ImageFile));
            if (!noteImageResponse.Ok)
                throw new("Call to IFileService.SaveImage() failed. " + noteImageResponse.ErrorMessages);
            if (!noteImageResponse.Value!.Id.HasValue)
                throw new("Image file is required.");
            noteImage.ImageFileId = noteImageResponse.Value.Id;

            await NoteImageInsert.Execute(connection, transaction, sessionId, noteImage);
        }

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var noteElement = element.RequireConfig<NoteElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageUpdateResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageUpdateResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageUpdateResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageUpdateResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var noteImageFileUpdateResponse = await fileService.SaveImage(new(sessionId, noteElement.ImageFileFile), noteElement.ImageFileFile.Id);
        if (!noteImageFileUpdateResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + noteImageFileUpdateResponse.ErrorMessages);
        if (!noteImageFileUpdateResponse.Value!.Id.HasValue)
            throw new("Image file is required.");
        noteElement.ImageFileFile.Id = noteImageFileUpdateResponse.Value.Id;

        await NoteUpdate.Execute(connection, transaction, sessionId, noteElement);

        var existingNoteImages = await NoteImageSelectForNote.Execute(connection, transaction, noteElement.Id);

        if (noteElement.RequireImageSelection != true)
            noteElement.NoteImages = [];
        else
            foreach (var noteImage in noteElement.NoteImages)
            {
                var existing = existingNoteImages.FirstOrDefault(x => x.ImageFileId.Equals(noteImage.ImageFileId));
                var noteImageResponse = await fileService.SaveImage(new(sessionId, noteImage.ImageFile), existing?.ImageFile);
                if (!noteImageResponse.Ok)
                    throw new("Call to IFileService.SaveImage() failed. " + noteImageResponse.ErrorMessages);
                if (!noteImageResponse.Value!.Id.HasValue)
                    throw new("Image file is required.");
                noteImage.ImageFileId = noteImageResponse.Value.Id;
            }

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existingNoteImages,
            noteElement.NoteImages,
            NoteImageInsert.Execute,
            NoteImageUpdate.Execute,
            NoteImageDelete.Execute);

        noteElement.NoteImages.EnsureOrder();
        await NoteImageUpdateOrdinalsByBatch.Execute(connection, transaction, sessionId, noteElement.NoteImages);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var noteElement = element.RequireConfig<NoteElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await NoteDelete.Execute(connection, transaction, sessionId, noteElement);
    }
}