namespace Crudspa.Education.Student.Server.Services;

public class StudentAchievementServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IStudentAchievementService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<StudentAchievement?>> CheckGame(Request<GameProgress> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var game = await GameSelectAchievements.Execute(Connection, progress.GameId);

            if (game?.GeneratesAchievement.Id is null)
                return null;

            var studentAchievement = new StudentAchievement
            {
                StudentId = progress.StudentId,
                RelatedEntityId = game.Id,
                Achievement = game.GeneratesAchievement,
            };

            var studentHasAchievement = await StudentAchievementExists.Execute(Connection, studentAchievement);

            if (studentHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                studentAchievement.Id = await StudentAchievementInsert.Execute(connection, transaction, request.SessionId, studentAchievement);
            });

            studentAchievement.IsNew = true;
            return studentAchievement;
        });
    }

    public async Task<Response<StudentAchievement?>> CheckLesson(Request<ObjectiveProgress> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var lesson = await LessonSelectAchievements.Execute(Connection, progress.ObjectiveId);

            if (lesson?.GeneratesAchievement?.Id is null)
                return null;

            var allAreComplete = await LessonAllObjectivesAreCompleted.Execute(Connection, request.SessionId, lesson.Id, null);

            if (!allAreComplete)
                return null;

            var studentAchievement = new StudentAchievement
            {
                StudentId = progress.StudentId,
                RelatedEntityId = lesson.Id,
                Achievement = lesson.GeneratesAchievement,
            };

            var studentHasAchievement = await StudentAchievementExists.Execute(Connection, studentAchievement);

            if (studentHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                studentAchievement.Id = await StudentAchievementInsert.Execute(connection, transaction, request.SessionId, studentAchievement);
            });

            studentAchievement.IsNew = true;
            return studentAchievement;
        });
    }

    public async Task<Response<StudentAchievement?>> CheckModule(Request<ModuleProgress> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var module = await ModuleSelectAchievements.Execute(Connection, progress.ModuleId);

            if (module?.GeneratesAchievement.Id is null)
                return null;

            var studentAchievement = new StudentAchievement
            {
                StudentId = progress.StudentId,
                RelatedEntityId = module.Id,
                Achievement = module.GeneratesAchievement,
            };

            var studentHasAchievement = await StudentAchievementExists.Execute(Connection, studentAchievement);

            if (studentHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                studentAchievement.Id = await StudentAchievementInsert.Execute(connection, transaction, request.SessionId, studentAchievement);
            });

            studentAchievement.IsNew = true;
            return studentAchievement;
        });
    }

    public async Task<Response<StudentAchievement?>> CheckObjective(Request<ObjectiveProgress> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var objective = await ObjectiveSelectAchievements.Execute(Connection, progress.ObjectiveId);

            if (objective?.GeneratesAchievement?.Id is null)
                return null;

            var studentAchievement = new StudentAchievement
            {
                StudentId = progress.StudentId,
                RelatedEntityId = objective.Id,
                Achievement = objective.GeneratesAchievement,
            };

            var studentHasAchievement = await StudentAchievementExists.Execute(Connection, studentAchievement);

            if (studentHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                studentAchievement.Id = await StudentAchievementInsert.Execute(connection, transaction, request.SessionId, studentAchievement);
            });

            studentAchievement.IsNew = true;
            return studentAchievement;
        });
    }

    public async Task<Response<StudentAchievement?>> CheckTrifold(Request<TrifoldProgress> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var trifold = await TrifoldSelectAchievements.Execute(Connection, progress.TrifoldId);

            if (trifold?.GeneratesAchievement?.Id is null)
                return null;

            var studentAchievement = new StudentAchievement
            {
                StudentId = progress.StudentId,
                RelatedEntityId = trifold.Id,
                Achievement = trifold.GeneratesAchievement,
            };

            var studentHasAchievement = await StudentAchievementExists.Execute(Connection, studentAchievement);

            if (studentHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                studentAchievement.Id = await StudentAchievementInsert.Execute(connection, transaction, request.SessionId, studentAchievement);
            });

            studentAchievement.IsNew = true;
            return studentAchievement;
        });
    }

    public async Task<Response<StudentAchievement?>> CheckUnit(Request<ObjectiveProgress> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var progress = request.Value;

            var unit = await UnitSelectAchievements.Execute(Connection, progress.ObjectiveId);

            if (unit?.GeneratesAchievement.Id is null)
                return null;

            var allLessonsAreComplete = await UnitAllLessonsAreComplete.Execute(Connection, request.SessionId, unit.Id);

            if (!allLessonsAreComplete)
                return null;

            var studentAchievement = new StudentAchievement
            {
                StudentId = progress.StudentId,
                RelatedEntityId = unit.Id,
                Achievement = unit.GeneratesAchievement,
            };

            var studentHasAchievement = await StudentAchievementExists.Execute(Connection, studentAchievement);

            if (studentHasAchievement)
                return null;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                studentAchievement.Id = await StudentAchievementInsert.Execute(connection, transaction, request.SessionId, studentAchievement);
            });

            studentAchievement.IsNew = true;
            return studentAchievement;
        });
    }
}