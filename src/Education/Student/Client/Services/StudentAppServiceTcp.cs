namespace Crudspa.Education.Student.Client.Services;

public class StudentAppServiceTcp(IProxyWrappers proxyWrappers) : IStudentAppService
{
    public async Task<Response> AcceptTerms(Request request) =>
        await proxyWrappers.Send("StudentAppAcceptTerms", request);

    public async Task<Response<Shared.Contracts.Data.Student>> FetchStudent(Request request) =>
        await proxyWrappers.Send<Shared.Contracts.Data.Student>("StudentAppFetchStudent", request);

    public async Task<Response> SaveProfile(Request<Shared.Contracts.Data.Student> request) =>
        await proxyWrappers.Send("StudentAppSaveProfile", request);

    public async Task<Response<BookLite?>> FetchBook(Request<Book> request) =>
        await proxyWrappers.Send<BookLite?>("StudentAppFetchBook", request);

    public async Task<Response<BookContent>> FetchChapters(Request<Book> request) =>
        await proxyWrappers.Send<BookContent>("StudentAppFetchChapters", request);

    public async Task<Response<Chapter?>> FetchChapter(Request<Chapter> request) =>
        await proxyWrappers.Send<Chapter?>("StudentAppFetchChapter", request);

    public async Task<Response<Game?>> FetchGame(Request<Game> request) =>
        await proxyWrappers.Send<Game?>("StudentAppFetchGame", request);

    public async Task<Response<Module?>> FetchModule(Request<Module> request) =>
        await proxyWrappers.Send<Module?>("StudentAppFetchModule", request);

    public async Task<Response<BookContent>> FetchTrifolds(Request<Book> request) =>
        await proxyWrappers.Send<BookContent>("StudentAppFetchTrifolds", request);

    public async Task<Response<Trifold?>> FetchTrifold(Request<Trifold> request) =>
        await proxyWrappers.Send<Trifold?>("StudentAppFetchTrifold", request);

    public async Task<Response<IList<Unit>>> FetchUnits(Request request) =>
        await proxyWrappers.Send<IList<Unit>>("StudentAppFetchUnits", request);

    public async Task<Response<Unit?>> FetchUnit(Request<Unit> request) =>
        await proxyWrappers.Send<Unit?>("StudentAppFetchUnit", request);

    public async Task<Response<Lesson?>> FetchLesson(Request<Lesson> request) =>
        await proxyWrappers.Send<Lesson?>("StudentAppFetchLesson", request);

    public async Task<Response<Objective?>> FetchObjective(Request<Objective> request) =>
        await proxyWrappers.Send<Objective?>("StudentAppFetchObjective", request);

    public async Task<Response> AddSurveyResponse(Request<AppSurveyResponse> request) =>
        await proxyWrappers.Send("StudentAppAddSurveyResponse", request);

    public async Task<Response<IList<StudentAchievement>>> FetchAchievements(Request request) =>
        await proxyWrappers.Send<IList<StudentAchievement>>("StudentAppFetchAchievements", request);

    public async Task<Response<StudentAchievement?>> FetchAchievement(Request<StudentAchievement> request) =>
        await proxyWrappers.Send<StudentAchievement?>("StudentAppFetchAchievement", request);
}