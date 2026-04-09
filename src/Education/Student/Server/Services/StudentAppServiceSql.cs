namespace Crudspa.Education.Student.Server.Services;

using Crudspa.Education.Student.Shared.Contracts.Behavior;
using Crudspa.Framework.Core.Server.Contracts.Behavior;
using Crudspa.Framework.Core.Shared.Contracts.Data;
using Shared.Contracts.Data;
using Shared.Contracts.Ids;
using Student = Shared.Contracts.Data.Student;

public class StudentAppServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IStudentAppService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response> AcceptTerms(Request request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StudentUpdateAcceptTerms.Execute(connection, transaction, request.SessionId);
            });
        });
    }

    public async Task<Response<Student>> FetchStudent(Request request)
    {
        return await wrappers.Try<Student>(request, async response =>
            (await StudentSelectSlimBySession.Execute(Connection, request.SessionId))!);
    }

    public async Task<Response> SaveProfile(Request<Student> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StudentUpdateProfile.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response<BookLite?>> FetchBook(Request<Book> request)
    {
        return await wrappers.Try<BookLite?>(request, async response =>
            await BookLiteSelect.Execute(Connection, request.Value, request.SessionId));
    }

    public async Task<Response<Chapter?>> FetchChapter(Request<Chapter> request)
    {
        return await wrappers.Try<Chapter?>(request, async response =>
            await ChapterSelect.Execute(Connection, request.Value, request.SessionId));
    }

    public async Task<Response<BookContent>> FetchChapters(Request<Book> request)
    {
        return await wrappers.Try<BookContent>(request, async response =>
        {
            var book = request.Value;

            var titleTask = BookTitleSelect.Execute(Connection, book.Id);
            var chaptersTask = ChapterSelectForBook.Execute(Connection, book.Id, request.SessionId);

            await Task.WhenAll(titleTask, chaptersTask);

            return new()
            {
                Id = book.Id,
                Title = await titleTask,
                Chapters = (await chaptersTask).ToObservable(),
            };
        });
    }

    public async Task<Response<Game?>> FetchGame(Request<Game> request)
    {
        return await wrappers.Try<Game?>(request, async response =>
        {
            var gameTask = GameExternalSelect.Execute(Connection, request.Value, request.SessionId);
            var studentTask = StudentSelectSlimBySession.Execute(Connection, request.SessionId);

            await Task.WhenAll(gameTask, studentTask);

            var game = await gameTask
                ?? throw new($"Game not found. Id: '{request.Value.Id:D}'.");

            var student = await studentTask
                ?? throw new($"No student found for session '{request.SessionId:D}'.");

            var currentAssignmentBatch = await AssignmentBatchSelectCurrent.Execute(Connection, student.Id, game.Id);

            Game assignedGame;

            if (currentAssignmentBatch is not null)
                assignedGame = await SelectActivitiesForAssignmentBatch(currentAssignmentBatch.Id);
            else
                assignedGame = await AssignNewActivities(request, student);

            game.GameRunId = assignedGame.GameRunId;
            game.GameActivities = assignedGame.GameActivities;

            return game;
        });
    }

    public async Task<Response<Module?>> FetchModule(Request<Module> request)
    {
        return await wrappers.Try<Module?>(request, async response =>
            await ModuleSelect.Execute(Connection, request.Value, request.SessionId));
    }

    public async Task<Response<Trifold?>> FetchTrifold(Request<Trifold> request)
    {
        return await wrappers.Try<Trifold?>(request, async response =>
            await TrifoldSelect.Execute(Connection, request.Value, request.SessionId));
    }

    public async Task<Response<BookContent>> FetchTrifolds(Request<Book> request)
    {
        return await wrappers.Try<BookContent>(request, async response =>
        {
            var book = request.Value;

            var titleTask = BookTitleSelect.Execute(Connection, book.Id);
            var trifoldsTask = TrifoldSelectForBook.Execute(Connection, book.Id, request.SessionId);

            await Task.WhenAll(titleTask, trifoldsTask);

            return new()
            {
                Id = book.Id,
                Title = await titleTask,
                Trifolds = (await trifoldsTask).ToObservable(),
            };
        });
    }

    public async Task<Response<IList<Unit>>> FetchUnits(Request request)
    {
        return await wrappers.Try<IList<Unit>>(request, async response =>
            await UnitSelectBySession.Execute(Connection, request.SessionId));
    }

    public async Task<Response<Unit?>> FetchUnit(Request<Unit> request)
    {
        return await wrappers.Try<Unit?>(request, async response =>
            await UnitSelect.Execute(Connection, request.Value.Id, request.SessionId));
    }

    public async Task<Response<Lesson?>> FetchLesson(Request<Lesson> request)
    {
        return await wrappers.Try<Lesson?>(request, async response =>
            await LessonSelect.Execute(Connection, request.Value.Id, request.SessionId));
    }

    public async Task<Response<Objective?>> FetchObjective(Request<Objective> request)
    {
        return await wrappers.Try<Objective?>(request, async response =>
            await ObjectiveSelect.Execute(Connection, request.Value, request.SessionId));
    }

    public async Task<Response> AddSurveyResponse(Request<AppSurveyResponse> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var appSurveyResponse = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AppSurveyResponseInsert.Execute(connection, transaction, request.SessionId, appSurveyResponse);
            });
        });
    }

    public async Task<Response<IList<StudentAchievement>>> FetchAchievements(Request request)
    {
        return await wrappers.Try<IList<StudentAchievement>>(request, async response =>
            await StudentAchievementSelectForSession.Execute(Connection, request.SessionId));
    }

    public async Task<Response<StudentAchievement?>> FetchAchievement(Request<StudentAchievement> request)
    {
        return await wrappers.Try<StudentAchievement?>(request, async response =>
        {
            var studentAchievement = await StudentAchievementSelect.Execute(Connection, request.SessionId, request.Value);

            if (studentAchievement is not null)
                studentAchievement.Unlocks = (await StudentAchievementSelectUnlocks.Execute(Connection, studentAchievement))!;

            return studentAchievement;
        });
    }

    #region Activity Assignment

    private async Task<Game> SelectActivitiesForAssignmentBatch(Guid? assignmentBatchId)
    {
        return new()
        {
            GameRunId = assignmentBatchId,
            GameActivities = (await GameActivitySelectForBatch.Execute(Connection, assignmentBatchId)).ToObservable(),
        };
    }

    private async Task<IList<GameSection>> SelectGameSectionsForGame(Request<Game> request) =>
        await GameSectionSelectForGame.Execute(Connection, request.Value.Id);

    private async Task<Game> AssignNewActivities(Request<Game> request, Student student)
    {
        var gameTask = GameSelect.Execute(Connection, request.Value);
        var gameSectionsTask = SelectGameSectionsForGame(request);

        await Task.WhenAll(gameTask, gameSectionsTask);

        var game = await gameTask
            ?? throw new($"Game not found. Id: '{request.Value.Id:D}'.");

        var gameSections = await gameSectionsTask;

        if (!gameSections.HasItems())
            throw new($"No sections found for game '{game.Id?.ToString("D")}'.");

        var assignmentBatch = BuildAssignmentBatch(game, gameSections, student);

        for (var i = 0; i < assignmentBatch.ActivityAssignments.Count; i++)
            assignmentBatch.ActivityAssignments[i].Ordinal = i;

        var assignmentBatchResponse = await AddAssignmentBatch(new(request.SessionId, assignmentBatch));

        if (assignmentBatchResponse.Ok)
        {
            game.GameRunId = assignmentBatchResponse.Value.Id;
            game.GameActivities = (await SelectActivitiesForAssignmentBatch(game.GameRunId)).GameActivities;
        }

        return game;
    }

    private static AssignmentBatch BuildAssignmentBatch(IUnique game, IEnumerable<GameSection> gameSections, Student student)
    {
        var assignmentBatch = new AssignmentBatch
        {
            GameId = game.Id,
            StudentId = student.Id,
            ActivityAssignments = [],
        };

        foreach (var section in gameSections.OrderBy(x => x.Ordinal))
        {
            var possibleActivities = section.GameActivities
                .Where(x => x.GroupId is null || x.GroupId.Equals(student.ResearchGroupId))
                .ToList();

            if (!possibleActivities.HasItems())
                continue;

            IList<GameActivity> activitiesToAdd;

            switch (section.TypeId)
            {
                case var id when id == GameSectionTypeIds.AllActivitiesInSpecifiedOrder:

                    activitiesToAdd = possibleActivities.OrderBy(x => x.Ordinal).ToList();

                    foreach (var gameActivity in activitiesToAdd)
                        assignmentBatch.ActivityAssignments.Add(new() { ActivityId = gameActivity.ActivityId });

                    break;

                case var id when id == GameSectionTypeIds.AllActivitiesInRandomOrder:

                    activitiesToAdd = possibleActivities.Shuffle();

                    foreach (var gameActivity in activitiesToAdd)
                        assignmentBatch.ActivityAssignments.Add(new() { ActivityId = gameActivity.ActivityId });

                    break;

                case var id when id == GameSectionTypeIds.OneActivityAtRandom:

                    activitiesToAdd = (List<GameActivity>)[possibleActivities.Shuffle()[0]];

                    foreach (var gameActivity in activitiesToAdd)
                        assignmentBatch.ActivityAssignments.Add(new() { ActivityId = gameActivity.ActivityId });

                    break;

                case var id when id == GameSectionTypeIds.TwoActivitiesAtRandom:

                    var shuffled = possibleActivities.Shuffle();

                    activitiesToAdd = (List<GameActivity>)[shuffled[0]];

                    if (shuffled.Count > 1)
                        activitiesToAdd.Add(shuffled[1]);

                    foreach (var gameActivity in activitiesToAdd)
                        assignmentBatch.ActivityAssignments.Add(new() { ActivityId = gameActivity.ActivityId });

                    break;

                default:
                    throw new($"GameSectionTypeId '{section.TypeId:D}' not supported.");
            }
        }

        return assignmentBatch;
    }

    private async Task<Response<AssignmentBatch>> AddAssignmentBatch(Request<AssignmentBatch> request)
    {
        return await wrappers.Try<AssignmentBatch>(request, async response =>
        {
            var assignmentBatch = request.Value;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                assignmentBatch.Id = await AssignmentBatchInsert.Execute(connection, transaction, request.SessionId, assignmentBatch);
                foreach (var activityAssignment in assignmentBatch.ActivityAssignments)
                {
                    activityAssignment.AssignmentBatchId = assignmentBatch.Id;
                    await ActivityAssignmentInsertStub.Execute(connection, transaction, request.SessionId, activityAssignment);
                }
            });

            return assignmentBatch;
        });
    }

    #endregion
}