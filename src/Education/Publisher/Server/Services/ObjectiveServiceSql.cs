using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class ObjectiveServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IBlobService blobService,
    IPagePartsService pagePartsService)
    : IObjectiveService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Objective>>> FetchForLesson(Request<Lesson> request)
    {
        return await wrappers.Try<IList<Objective>>(request, async response =>
        {
            var objectives = await ObjectiveSelectForLesson.Execute(Connection, request.SessionId, request.Value.Id);

            return objectives;
        });
    }

    public async Task<Response<Objective?>> Fetch(Request<Objective> request)
    {
        return await wrappers.Try<Objective?>(request, async response =>
        {
            var objective = await ObjectiveSelect.Execute(Connection, request.SessionId, request.Value);

            return objective;
        });
    }

    public async Task<Response<Objective?>> Add(Request<Objective> request)
    {
        return await wrappers.Validate<Objective?, Objective>(request, async response =>
        {
            var objective = request.Value;

            var trophyImageFileResponse = await fileService.SaveImage(new(request.SessionId, objective.TrophyImageFile));
            if (!trophyImageFileResponse.Ok)
            {
                response.AddErrors(trophyImageFileResponse.Errors);
                return null;
            }

            objective.TrophyImageFile.Id = trophyImageFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ObjectiveInsert.Execute(connection, transaction, request.SessionId, objective);

                return new Objective
                {
                    Id = id,
                    LessonId = objective.LessonId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Objective> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var objective = request.Value;

            var existing = await ObjectiveSelect.Execute(Connection, request.SessionId, objective);

            var trophyImageFileResponse = await fileService.SaveImage(new(request.SessionId, objective.TrophyImageFile), existing?.TrophyImageFile);
            if (!trophyImageFileResponse.Ok)
            {
                response.AddErrors(trophyImageFileResponse.Errors);
                return;
            }

            objective.TrophyImageFile.Id = trophyImageFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ObjectiveUpdate.Execute(connection, transaction, request.SessionId, objective);
            });
        });
    }

    public async Task<Response> Remove(Request<Objective> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var objective = request.Value;
            var existing = await ObjectiveSelect.Execute(Connection, request.SessionId, objective);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ObjectiveDelete.Execute(connection, transaction, request.SessionId, objective);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Objective>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var objectives = request.Value;

            objectives.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ObjectiveUpdateOrdinals.Execute(connection, transaction, request.SessionId, objectives);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BinderTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<Copy>> Copy(Request<Copy> request)
    {
        return await wrappers.Validate<Copy, Copy>(request, async response =>
        {
            var objectiveRequest = new Objective { Id = request.Value.ExistingId };

            var newObjective = await ObjectiveSelect.Execute(Connection, request.SessionId, objectiveRequest);

            if (newObjective is not null)
            {
                newObjective.Id = Guid.NewGuid();
                newObjective.LessonId = request.Value.ExistingParentId;
                newObjective.Title = request.Value.NewName;
                newObjective.TrophyImageFile.OptimizedBlobId = null;

                if (newObjective.TrophyImageFile.BlobId is not null)
                    newObjective.TrophyImageFile.BlobId = await blobService.Copy(newObjective.TrophyImageFile.BlobId.Value);

                newObjective.TrophyImageFile.Id = null;

                var newObjectiveResponse = await Add(new(request.SessionId, newObjective));

                if (newObjectiveResponse.Value is not null && newObjectiveResponse.Ok)
                {
                    var newObjectiveSelect = await Fetch(new(request.SessionId, newObjectiveResponse.Value));

                    if (newObjectiveSelect.Value is not null && newObjectiveSelect.Ok)
                    {
                        var pagesResponse = pagePartsService.FetchPages(request.SessionId, newObjective.Binder.Id);

                        if (pagesResponse.Result.Value is not null)
                        {
                            foreach (var page in pagesResponse.Result.Value.OrderBy(x => x.Ordinal))
                            {
                                if (newObjectiveResponse.Value is not null)
                                {
                                    var pageCopy = new Copy
                                    {
                                        ExistingId = page.Id,
                                        ExistingParentId = newObjectiveSelect.Value.Binder.Id,
                                        NewName = page.Title,
                                    };
                                    await pagePartsService.CopyPage(request.SessionId, pageCopy);
                                }
                            }
                        }
                    }
                }
            }

            return new();
        });
    }

    public async Task<Response<IList<Page>>> FetchPages(Request<ObjectivePage> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);

            if (binderId.HasNothing())
                throw new("Objective binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<ObjectivePage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Objective page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, binderId, page);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<ObjectivePage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Objective binder not found.");

            var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<ObjectivePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Objective page not found.");

            var saveResponse = await pagePartsService.SavePage(request.SessionId, binderId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<ObjectivePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Objective page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<ObjectivePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pages = request.Value.Pages;

            if (!pages.HasItems() || binderId.HasNothing())
                throw new("Objective binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<ObjectiveSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pageId = request.Value.PageId;

            if (pageId.HasNothing() || binderId.HasNothing())
                throw new("Objective page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, binderId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<ObjectiveSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Objective section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<ObjectiveSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Objective page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<ObjectiveSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Objective section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<ObjectiveSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Objective section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<ObjectiveSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.ObjectiveId);
            var pageId = request.Value.PageId;
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing() || binderId.HasNothing())
                throw new("Objective page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, binderId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, Guid? objectiveId)
    {
        var objective = await ObjectiveSelect.Execute(Connection, sessionId, new() { Id = objectiveId });
        return objective?.Binder?.Id;
    }
}