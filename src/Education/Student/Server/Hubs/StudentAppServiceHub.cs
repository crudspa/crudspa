namespace Crudspa.Education.Student.Server.Hubs;

public partial class StudentHub
{
    public async Task<Response> StudentAppAcceptTerms(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.AcceptTerms(request));
    }

    public async Task<Response<Shared.Contracts.Data.Student>> StudentAppFetchStudent(Request request)
    {
        return await HubWrappers.RequireSession(request, async session =>
            await StudentAppService.FetchStudent(request));
    }

    public async Task<Response> StudentAppSaveProfile(Request<Shared.Contracts.Data.Student> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.SaveProfile(request));
    }

    public async Task<Response<BookLite?>> StudentAppFetchBook(Request<Book> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchBook(request));
    }

    public async Task<Response<BookContent>> StudentAppFetchChapters(Request<Book> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchChapters(request));
    }

    public async Task<Response<Chapter?>> StudentAppFetchChapter(Request<Chapter> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchChapter(request));
    }

    public async Task<Response<Game?>> StudentAppFetchGame(Request<Game> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchGame(request));
    }

    public async Task<Response<Module?>> StudentAppFetchModule(Request<Module> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchModule(request));
    }

    public async Task<Response<BookContent>> StudentAppFetchTrifolds(Request<Book> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchTrifolds(request));
    }

    public async Task<Response<Trifold?>> StudentAppFetchTrifold(Request<Trifold> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchTrifold(request));
    }

    public async Task<Response<IList<Unit>>> StudentAppFetchUnits(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchUnits(request));
    }

    public async Task<Response<Unit?>> StudentAppFetchUnit(Request<Unit> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchUnit(request));
    }

    public async Task<Response<Lesson?>> StudentAppFetchLesson(Request<Lesson> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchLesson(request));
    }

    public async Task<Response<Objective?>> StudentAppFetchObjective(Request<Objective> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchObjective(request));
    }

    public async Task<Response> StudentAppAddSurveyResponse(Request<AppSurveyResponse> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.AddSurveyResponse(request));
    }

    public async Task<Response<IList<StudentAchievement>>> StudentAppFetchAchievements(Request request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchAchievements(request));
    }

    public async Task<Response<StudentAchievement?>> StudentAppFetchAchievement(Request<StudentAchievement> request)
    {
        return await HubWrappers.RequireUser(request, async session =>
            await StudentAppService.FetchAchievement(request));
    }
}