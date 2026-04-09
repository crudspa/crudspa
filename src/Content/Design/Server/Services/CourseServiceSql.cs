namespace Crudspa.Content.Design.Server.Services;

public class CourseServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer,
    IPagePartsService pagePartsService)
    : ICourseService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Course>>> FetchForTrack(Request<Track> request)
    {
        return await wrappers.Try<IList<Course>>(request, async response =>
        {
            var courses = await CourseSelectForTrack.Execute(Connection, request.SessionId, request.Value.Id);
            return courses;
        });
    }

    public async Task<Response<Course?>> Fetch(Request<Course> request)
    {
        return await wrappers.Try<Course?>(request, async response =>
        {
            var course = await CourseSelect.Execute(Connection, request.SessionId, request.Value);
            return course;
        });
    }

    public async Task<Response<Course?>> Add(Request<Course> request)
    {
        return await wrappers.Validate<Course?, Course>(request, async response =>
        {
            var course = request.Value;

            course.Description = htmlSanitizer.Sanitize(course.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await CourseInsert.Execute(connection, transaction, request.SessionId, course);

                return new Course
                {
                    Id = id,
                    TrackId = course.TrackId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Course> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var course = request.Value;

            course.Description = htmlSanitizer.Sanitize(course.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CourseUpdate.Execute(connection, transaction, request.SessionId, course);
            });
        });
    }

    public async Task<Response> Remove(Request<Course> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var course = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CourseDelete.Execute(connection, transaction, request.SessionId, course);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Course>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var courses = request.Value;

            courses.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await CourseUpdateOrdinals.Execute(connection, transaction, request.SessionId, courses);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection, request.Value.Id));
    }

    public async Task<Response<IList<Orderable>>> FetchBinderTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await BinderTypeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Page>>> FetchPages(Request<CoursePage> request)
    {
        return await wrappers.Try<IList<Page>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);

            if (binderId.HasNothing())
                throw new("Course binder not found.");

            var fetchResponse = await pagePartsService.FetchPages(request.SessionId, binderId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> FetchPage(Request<CoursePage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Course page not found.");

            var fetchResponse = await pagePartsService.FetchPage(request.SessionId, binderId, page);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Page?>> AddPage(Request<CoursePage> request)
    {
        return await wrappers.Try<Page?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Course binder not found.");

            var addResponse = await pagePartsService.AddPage(request.SessionId, binderId, page);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SavePage(Request<CoursePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Course page not found.");

            var saveResponse = await pagePartsService.SavePage(request.SessionId, binderId, page);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemovePage(Request<CoursePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var page = request.Value.Page;

            if (page is null || binderId.HasNothing())
                throw new("Course page not found.");

            var removeResponse = await pagePartsService.RemovePage(request.SessionId, binderId, page);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SavePageOrder(Request<CoursePage> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pages = request.Value.Pages;

            if (!pages.HasItems() || binderId.HasNothing())
                throw new("Course binder not found.");

            var saveResponse = await pagePartsService.SavePageOrder(request.SessionId, binderId, pages);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response<IList<Section>>> FetchSections(Request<CourseSection> request)
    {
        return await wrappers.Try<IList<Section>>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pageId = request.Value.PageId;

            if (pageId.HasNothing() || binderId.HasNothing())
                throw new("Course page not found.");

            var fetchResponse = await pagePartsService.FetchSections(request.SessionId, binderId, pageId);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> FetchSection(Request<CourseSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Course section not found.");

            var fetchResponse = await pagePartsService.FetchSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(fetchResponse.Errors);
            return fetchResponse.Value!;
        });
    }

    public async Task<Response<Section?>> AddSection(Request<CourseSection> request)
    {
        return await wrappers.Try<Section?>(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Course page not found.");

            var addResponse = await pagePartsService.AddSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(addResponse.Errors);
            return addResponse.Value!;
        });
    }

    public async Task<Response> SaveSection(Request<CourseSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Course section not found.");

            var saveResponse = await pagePartsService.SaveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(saveResponse.Errors);
        });
    }

    public async Task<Response> RemoveSection(Request<CourseSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pageId = request.Value.PageId;
            var section = request.Value.Section;

            if (section is null || pageId.HasNothing() || binderId.HasNothing())
                throw new("Course section not found.");

            var removeResponse = await pagePartsService.RemoveSection(request.SessionId, binderId, pageId, section);
            response.AddErrors(removeResponse.Errors);
        });
    }

    public async Task<Response> SaveSectionOrder(Request<CourseSection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var binderId = await FetchBinderId(request.SessionId, request.Value.CourseId);
            var pageId = request.Value.PageId;
            var sections = request.Value.Sections;

            if (!sections.HasItems() || pageId.HasNothing() || binderId.HasNothing())
                throw new("Course page not found.");

            var saveResponse = await pagePartsService.SaveSectionOrder(request.SessionId, binderId, pageId, sections);
            response.AddErrors(saveResponse.Errors);
        });
    }

    private async Task<Guid?> FetchBinderId(Guid? sessionId, Guid? courseId)
    {
        var course = await CourseSelect.Execute(Connection, sessionId, new() { Id = courseId });
        return course?.Binder?.Id;
    }
}