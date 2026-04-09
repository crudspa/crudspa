namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IStudentAppService
{
    Task<Response> AcceptTerms(Request request);
    Task<Response<Data.Student>> FetchStudent(Request request);
    Task<Response> SaveProfile(Request<Data.Student> request);
    Task<Response<BookLite?>> FetchBook(Request<Book> request);
    Task<Response<BookContent>> FetchChapters(Request<Book> request);
    Task<Response<Chapter?>> FetchChapter(Request<Chapter> request);
    Task<Response<Game?>> FetchGame(Request<Game> request);
    Task<Response<Module?>> FetchModule(Request<Module> request);
    Task<Response<BookContent>> FetchTrifolds(Request<Book> request);
    Task<Response<Trifold?>> FetchTrifold(Request<Trifold> request);
    Task<Response<IList<Unit>>> FetchUnits(Request request);
    Task<Response<Unit?>> FetchUnit(Request<Unit> request);
    Task<Response<Lesson?>> FetchLesson(Request<Lesson> request);
    Task<Response<Objective?>> FetchObjective(Request<Objective> request);
    Task<Response> AddSurveyResponse(Request<AppSurveyResponse> request);
    Task<Response<IList<StudentAchievement>>> FetchAchievements(Request request);
    Task<Response<StudentAchievement?>> FetchAchievement(Request<StudentAchievement> request);
}