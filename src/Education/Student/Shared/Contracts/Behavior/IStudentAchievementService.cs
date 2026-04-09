namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IStudentAchievementService
{
    Task<Response<StudentAchievement?>> CheckGame(Request<GameProgress> request);
    Task<Response<StudentAchievement?>> CheckLesson(Request<ObjectiveProgress> request);
    Task<Response<StudentAchievement?>> CheckModule(Request<ModuleProgress> request);
    Task<Response<StudentAchievement?>> CheckObjective(Request<ObjectiveProgress> request);
    Task<Response<StudentAchievement?>> CheckTrifold(Request<TrifoldProgress> request);
    Task<Response<StudentAchievement?>> CheckUnit(Request<ObjectiveProgress> request);
}