namespace Crudspa.Content.Design.Server.Services;

public class SectionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IElementRepositoryFactory elementRepositoryFactory,
    IFileService fileService,
    IPageRunService pageRunService,
    ISectionRepository sectionRepository)
    : ISectionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Section>>> FetchForPage(Request<Page> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var pageRunResponse = await pageRunService.Fetch(request);

            if (pageRunResponse.Ok)
                return pageRunResponse.Value.Sections;

            response.AddErrors(pageRunResponse.Errors);

            return null!;
        });
    }

    public async Task<Response<Section?>> Fetch(Request<Section> request)
    {
        return await wrappers.Try<Section?>(request, async response => await FetchSection(request));
    }

    public async Task<Response<Section?>> Add(Request<Section> request)
    {
        return await wrappers.Validate<Section?, Section>(request, async response =>
        {
            var section = request.Value;

            var errors = new List<Error>();

            foreach (var element in section.Elements)
            {
                var elementRepository = elementRepositoryFactory.Create(element.ElementType!.RepositoryClass!);
                errors.AddRange(await elementRepository.Validate(Connection, element));
            }

            if (errors.HasItems())
            {
                response.AddErrors(errors);
                return null;
            }

            section.Elements.EnsureOrder();

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var backgroundImageResponse = await fileService.SaveImage(new(request.SessionId, section.Box.BackgroundImageFile));
                if (!backgroundImageResponse.Ok)
                    throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
                section.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

                section.Box.Id = await BoxUpsert.Execute(connection, transaction, request.SessionId, section.Box);
                section.Container.Id = await ContainerUpsert.Execute(connection, transaction, request.SessionId, section.Container);

                section.Id = await sectionRepository.Insert(connection, transaction, request.SessionId, section);

                foreach (var element in section.Elements)
                {
                    element.SectionId = section.Id;
                    var elementRepository = elementRepositoryFactory.Create(element.ElementType!.RepositoryClass!);
                    await elementRepository.Insert(connection, transaction, request.SessionId, element);
                }
            });

            return new()
            {
                Id = section.Id,
                PageId = section.PageId,
            };
        });
    }

    public async Task<Response> Save(Request<Section> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var section = request.Value;

            var errors = new List<Error>();

            foreach (var element in section.Elements)
            {
                var elementRepository = elementRepositoryFactory.Create(element.ElementType!.RepositoryClass!);
                errors.AddRange(await elementRepository.Validate(Connection, element));
            }

            if (errors.HasItems())
            {
                response.AddErrors(errors);
                return;
            }

            section.Elements.EnsureOrder();

            var existingSection = await FetchSection(new(request.SessionId, section));
            var existingElements = existingSection?.Elements ?? [];

            var existingBox = await BoxSelect.Execute(Connection, request.SessionId, section.Box);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var backgroundImageResponse = await fileService.SaveImage(new(request.SessionId, section.Box.BackgroundImageFile), existingBox?.BackgroundImageFile);
                if (!backgroundImageResponse.Ok)
                    throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
                section.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

                section.Box.Id = await BoxUpsert.Execute(connection, transaction, request.SessionId, section.Box);
                section.Container.Id = await ContainerUpsert.Execute(connection, transaction, request.SessionId, section.Container);

                await sectionRepository.Update(connection, transaction, request.SessionId, section);

                foreach (var updated in section.Elements)
                {
                    var elementRepository = elementRepositoryFactory.Create(updated.ElementType!.RepositoryClass!);

                    var existing = existingElements.FirstOrDefault(x => x.Id.Equals(updated.Id));

                    if (existing is null)
                        updated.Id = await elementRepository.Insert(connection, transaction, request.SessionId, updated);
                    else
                        await elementRepository.Update(connection, transaction, request.SessionId, updated);
                }

                foreach (var existing in existingElements.Where(existing => section.Elements.All(x => x.Id != existing.Id)))
                {
                    var elementRepository = elementRepositoryFactory.Create(existing.ElementType!.RepositoryClass!);
                    await elementRepository.Delete(connection, transaction, request.SessionId, existing);
                }
            });
        });
    }

    public async Task<Response> Remove(Request<Section> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var section = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await sectionRepository.Delete(connection, transaction, request.SessionId, section);
            });
        });
    }

    public async Task<Response<Copy>> Copy(Request<Copy> request)
    {
        return await wrappers.Validate<Copy, Copy>(request, async response =>
        {
            var newSection = await FetchSection(new(request.SessionId, new() { Id = request.Value.ExistingId, PageId = request.Value.ExistingParentId }));

            if (newSection is not null)
            {
                newSection.Id = Guid.NewGuid();
                newSection.PageId = request.Value.NewParentId;

                var newSectionResponse = await Add(new(request.SessionId, newSection));

                if (!newSectionResponse.Ok)
                    throw new("Add failed.");
            }

            return new();
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Section>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var sections = request.Value;

            sections.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await sectionRepository.SaveOrder(connection, transaction, request.SessionId, sections);
            });
        });
    }

    public async Task<Response<IList<ElementType>>> FetchElementTypes(Request request)
    {
        return await wrappers.Try<IList<ElementType>>(request, async response =>
            await ElementTypeSelectFull.Execute(Connection));
    }

    public async Task<Response<SectionElement?>> CreateElement(Request<ElementSpec> request)
    {
        return await wrappers.Try<SectionElement?>(request, async response =>
        {
            var spec = request.Value;

            var elementRepository = elementRepositoryFactory.Create(spec.ElementType.RepositoryClass!);

            return await elementRepository.Create(spec.ElementType, spec.SectionId, spec.Ordinal);
        });
    }

    private async Task<Section?> FetchSection(Request<Section> request)
    {
        var section = request.Value;

        var page = new Page { Id = section.PageId };

        var pageRunResponse = await pageRunService.Fetch(new(request.SessionId, page));

        if (pageRunResponse.Ok)
            return pageRunResponse.Value.Sections.FirstOrDefault(x => x.Id.Equals(section.Id));

        return null!;
    }
}