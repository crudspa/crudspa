namespace Crudspa.Content.Design.Server.Services;

public class PagePartsServiceSql(
    IServerConfigService configService,
    ISqlWrappers sqlWrappers,
    IFileService fileService,
    IBlobService blobService,
    ISectionService sectionService,
    IBinderRepository binderRepository,
    IPageRepository pageRepository)
    : IPagePartsService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Page>>> FetchPages(Guid? sessionId, Guid? binderId)
    {
        if (binderId.HasNothing())
            return new("Page not found.");

        return new(await pageRepository.SelectForBinder(Connection, sessionId, binderId));
    }

    public async Task<Response<Page?>> FetchPage(Guid? sessionId, Page page)
    {
        if (page.Id.HasNothing())
            return new("Page not found.");

        var existing = await pageRepository.Select(Connection, sessionId, page);
        return existing is null ? new("Page not found.") : new(existing);
    }

    public async Task<Response<Page?>> FetchPage(Guid? sessionId, Guid? binderId, Page page)
    {
        if (!await MatchesBinder(sessionId, binderId, page.Id))
            return new("Page not found.");

        return await FetchPage(sessionId, page);
    }

    public async Task<Response<Page?>> AddPage(Guid? sessionId, Page page)
    {
        var errors = page.Validate();

        if (errors.HasItems())
            return new() { Errors = errors };

        var guideAudioFileResponse = await fileService.SaveAudio(new(sessionId, page.GuideAudioFile));

        if (!guideAudioFileResponse.Ok)
            return new() { Errors = guideAudioFileResponse.Errors };

        page.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

        var createdPage = await sqlWrappers.WithTransaction(async (connection, transaction) =>
        {
            var backgroundImageResponse = await fileService.SaveImage(new(sessionId, page.Box.BackgroundImageFile));

            if (!backgroundImageResponse.Ok)
                throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);

            page.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;
            page.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, page.Box);

            var id = await pageRepository.Insert(connection, transaction, sessionId, page);

            return new Page
            {
                Id = id,
                BinderId = page.BinderId,
            };
        });

        return new(createdPage);
    }

    public async Task<Response<Page?>> AddPage(Guid? sessionId, Guid? binderId, Page page)
    {
        if (binderId.HasNothing())
            return new("Page not found.");

        page.BinderId = binderId;
        return await AddPage(sessionId, page);
    }

    public async Task<Response<Copy>> CopyPage(Guid? sessionId, Copy copy)
    {
        var errors = copy.Validate();

        if (errors.HasItems())
            return new() { Errors = errors };

        var pageRequest = new Page { Id = copy.ExistingId };
        var newPage = await pageRepository.Select(Connection, sessionId, pageRequest);

        if (newPage is not null)
        {
            newPage.Id = Guid.NewGuid();
            newPage.Title = copy.NewName;
            newPage.GuideAudioFile.OptimizedBlobId = null;
            newPage.BinderId = copy.ExistingParentId;

            if (newPage.GuideAudioFile.BlobId is not null)
                newPage.GuideAudioFile.BlobId = await blobService.Copy(newPage.GuideAudioFile.BlobId.Value);

            newPage.GuideAudioFile.Id = null;

            var newPageResponse = await AddPage(sessionId, newPage);

            if (!newPageResponse.Ok)
                return new() { Errors = newPageResponse.Errors };

            var sectionResponse = await sectionService.FetchForPage(new(sessionId, new() { Id = pageRequest.Id }));

            if (sectionResponse.Ok && sectionResponse.Value is not null)
            {
                foreach (var section in sectionResponse.Value.OrderBy(x => x.Ordinal))
                {
                    var sectionCopy = new Copy
                    {
                        ExistingId = section.Id,
                        ExistingParentId = pageRequest.Id,
                        NewParentId = newPageResponse.Value.Id,
                        NewName = "Section Name",
                        NewId = Guid.NewGuid(),
                    };

                    await sectionService.Copy(new(sessionId, sectionCopy));
                }
            }
        }

        return new();
    }

    public async Task<Response> SavePage(Guid? sessionId, Page page)
    {
        var errors = page.Validate();

        if (errors.HasItems())
            return new() { Errors = errors };

        if (page.Id.HasNothing())
            return new("Page not found.");

        var existing = await pageRepository.Select(Connection, sessionId, page);
        var existingBox = await BoxSelect.Execute(Connection, sessionId, page.Box);

        if (existing is null)
            return new("Page not found.");

        var guideAudioFileResponse = await fileService.SaveAudio(new(sessionId, page.GuideAudioFile), existing.GuideAudioFile);

        if (!guideAudioFileResponse.Ok)
            return new() { Errors = guideAudioFileResponse.Errors };

        page.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

        await sqlWrappers.WithTransaction(async (connection, transaction) =>
        {
            var backgroundImageResponse = await fileService.SaveImage(new(sessionId, page.Box.BackgroundImageFile), existingBox?.BackgroundImageFile);

            if (!backgroundImageResponse.Ok)
                throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);

            page.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;
            page.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, page.Box);

            await pageRepository.Update(connection, transaction, sessionId, page);
        });

        return new();
    }

    public async Task<Response> SavePage(Guid? sessionId, Guid? binderId, Page page)
    {
        if (!await MatchesBinder(sessionId, binderId, page.Id))
            return new("Page not found.");

        page.BinderId = binderId;
        return await SavePage(sessionId, page);
    }

    public async Task<Response> RemovePage(Guid? sessionId, Page page)
    {
        if (page.Id.HasNothing())
            return new("Page not found.");

        var existing = await pageRepository.Select(Connection, sessionId, page);

        if (existing is null)
            return new("Page not found.");

        await sqlWrappers.WithConnection(async (connection, transaction) =>
        {
            await pageRepository.Delete(connection, transaction, sessionId, page);
        });

        return new();
    }

    public async Task<Response> RemovePage(Guid? sessionId, Guid? binderId, Page page)
    {
        if (!await MatchesBinder(sessionId, binderId, page.Id))
            return new("Page not found.");

        page.BinderId = binderId;
        return await RemovePage(sessionId, page);
    }

    public async Task<Response> SavePageOrder(Guid? sessionId, Guid? binderId, IList<Page> pages)
    {
        if (binderId.HasNothing() || !await MatchesPages(sessionId, binderId, pages))
            return new("Page not found.");

        foreach (var page in pages)
            page.BinderId = binderId;

        pages.EnsureOrder();

        await sqlWrappers.WithConnection(async (connection, transaction) =>
        {
            await pageRepository.SaveOrder(connection, transaction, sessionId, pages);
        });

        return new();
    }

    public async Task<Response<Binder?>> AddBinder(Guid? sessionId, Binder binder)
    {
        var binderResponse = await sqlWrappers.WithConnection(async (connection, transaction) =>
        {
            var id = await binderRepository.Insert(connection, transaction, sessionId, binder);
            return new Binder { Id = id };
        });

        return new(binderResponse);
    }

    public async Task<Response<IList<Section>>> FetchSections(Guid? sessionId, Guid? pageId)
    {
        if (!await HasPage(sessionId, pageId))
            return new("Page not found.");

        return await sectionService.FetchForPage(new(sessionId, new() { Id = pageId }));
    }

    public async Task<Response<IList<Section>>> FetchSections(Guid? sessionId, Guid? binderId, Guid? pageId)
    {
        if (!await MatchesBinder(sessionId, binderId, pageId))
            return new("Page not found.");

        return await FetchSections(sessionId, pageId);
    }

    public async Task<Response<Section?>> FetchSection(Guid? sessionId, Guid? pageId, Section section)
    {
        if (!await MatchesSection(sessionId, pageId, section.Id))
            return new("Section not found.");

        section.PageId = pageId;

        var response = await sectionService.Fetch(new(sessionId, section));
        return response.Ok && response.Value is null ? new("Section not found.") : response;
    }

    public async Task<Response<Section?>> FetchSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section)
    {
        if (!await MatchesBinder(sessionId, binderId, pageId))
            return new("Page not found.");

        return await FetchSection(sessionId, pageId, section);
    }

    public async Task<Response<Section?>> AddSection(Guid? sessionId, Guid? pageId, Section section)
    {
        if (!await HasPage(sessionId, pageId))
            return new("Page not found.");

        section.PageId = pageId;
        return await sectionService.Add(new(sessionId, section));
    }

    public async Task<Response<Section?>> AddSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section)
    {
        if (!await MatchesBinder(sessionId, binderId, pageId))
            return new("Page not found.");

        return await AddSection(sessionId, pageId, section);
    }

    public async Task<Response> SaveSection(Guid? sessionId, Guid? pageId, Section section)
    {
        if (!await MatchesSection(sessionId, pageId, section.Id))
            return new("Section not found.");

        section.PageId = pageId;
        return await sectionService.Save(new(sessionId, section));
    }

    public async Task<Response> SaveSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section)
    {
        if (!await MatchesBinder(sessionId, binderId, pageId))
            return new("Page not found.");

        return await SaveSection(sessionId, pageId, section);
    }

    public async Task<Response> RemoveSection(Guid? sessionId, Guid? pageId, Section section)
    {
        if (!await MatchesSection(sessionId, pageId, section.Id))
            return new("Section not found.");

        section.PageId = pageId;
        return await sectionService.Remove(new(sessionId, section));
    }

    public async Task<Response> RemoveSection(Guid? sessionId, Guid? binderId, Guid? pageId, Section section)
    {
        if (!await MatchesBinder(sessionId, binderId, pageId))
            return new("Page not found.");

        return await RemoveSection(sessionId, pageId, section);
    }

    public async Task<Response> SaveSectionOrder(Guid? sessionId, Guid? pageId, IList<Section> sections)
    {
        if (!await MatchesSections(sessionId, pageId, sections))
            return new("Section not found.");

        foreach (var section in sections)
            section.PageId = pageId;

        return await sectionService.SaveOrder(new(sessionId, sections));
    }

    public async Task<Response> SaveSectionOrder(Guid? sessionId, Guid? binderId, Guid? pageId, IList<Section> sections)
    {
        if (!await MatchesBinder(sessionId, binderId, pageId))
            return new("Page not found.");

        return await SaveSectionOrder(sessionId, pageId, sections);
    }

    private async Task<Boolean> HasPage(Guid? sessionId, Guid? pageId)
    {
        if (pageId.HasNothing())
            return false;

        return await pageRepository.Select(Connection, sessionId, new() { Id = pageId }) is not null;
    }

    private async Task<Boolean> MatchesBinder(Guid? sessionId, Guid? binderId, Guid? pageId)
    {
        if (binderId.HasNothing() || pageId.HasNothing())
            return false;

        var existing = await pageRepository.Select(Connection, sessionId, new() { Id = pageId });
        return existing?.BinderId.Equals(binderId) == true;
    }

    private async Task<Boolean> MatchesPages(Guid? sessionId, Guid? binderId, IList<Page> pages)
    {
        if (binderId.HasNothing() || !pages.HasItems())
            return false;

        var ids = (await pageRepository.SelectForBinder(Connection, sessionId, binderId))
            .Select(x => x.Id)
            .ToHashSet();

        return pages.All(x => x.Id.HasSomething() && ids.Contains(x.Id));
    }

    private async Task<Boolean> MatchesSection(Guid? sessionId, Guid? pageId, Guid? sectionId)
    {
        if (pageId.HasNothing() || sectionId.HasNothing())
            return false;

        var response = await sectionService.Fetch(new(sessionId, new()
        {
            Id = sectionId,
            PageId = pageId,
        }));

        return response.Ok && response.Value is not null;
    }

    private async Task<Boolean> MatchesSections(Guid? sessionId, Guid? pageId, IList<Section> sections)
    {
        if (pageId.HasNothing() || !sections.HasItems())
            return false;

        var response = await sectionService.FetchForPage(new(sessionId, new() { Id = pageId }));

        if (!response.Ok)
            return false;

        var ids = response.Value.Select(x => x.Id).ToHashSet();
        return sections.All(x => x.Id.HasSomething() && ids.Contains(x.Id));
    }
}