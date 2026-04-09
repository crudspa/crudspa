namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryPdf(IServerConfigService configService, IFileService fileService) : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var pdfElementId = Guid.NewGuid();
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

        sectionElement.SetConfig(new PdfElement
        {
            Id = pdfElementId,
            ElementId = elementId,
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var pdfElement = element.RequireConfig<PdfElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(pdfElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var pdfElement = element.RequireConfig<PdfElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        pdfElement.ElementId = elementId;

        var pdfResponse = await fileService.SavePdf(new(sessionId, pdfElement.FileFile));
        if (!pdfResponse.Ok)
            throw new("Call to IFileService.SavePdf() failed. " + pdfResponse.ErrorMessages);
        if (!pdfResponse.Value.Id.HasValue)
            throw new("PDF file is required.");

        pdfElement.FileFile.Id = pdfResponse.Value.Id;

        await PdfInsert.Execute(connection, transaction, sessionId, pdfElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var pdfElement = element.RequireConfig<PdfElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        var pdfResponse = await fileService.SavePdf(new(sessionId, pdfElement.FileFile), pdfElement.FileFile.Id);
        if (!pdfResponse.Ok)
            throw new("Call to IFileService.SavePdf() failed. " + pdfResponse.ErrorMessages);
        if (!pdfResponse.Value.Id.HasValue)
            throw new("PDF file is required.");

        pdfElement.FileFile.Id = pdfResponse.Value.Id;

        await PdfUpdate.Execute(connection, transaction, sessionId, pdfElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var pdfElement = element.RequireConfig<PdfElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await PdfDelete.Execute(connection, transaction, sessionId, pdfElement);
    }
}