using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class LessonServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IBlobService blobService,
    IObjectiveService objectiveService)
    : ILessonService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Lesson>>> FetchForUnit(Request<Unit> request)
    {
        return await wrappers.Try<IList<Lesson>>(request, async response =>
        {
            var lessons = await LessonSelectForUnit.Execute(Connection, request.SessionId, request.Value.Id);
            return lessons;
        });
    }

    public async Task<Response<Lesson?>> Fetch(Request<Lesson> request)
    {
        return await wrappers.Try<Lesson?>(request, async response =>
        {
            var lesson = await LessonSelect.Execute(Connection, request.SessionId, request.Value);
            return lesson;
        });
    }

    public async Task<Response<Lesson?>> Add(Request<Lesson> request)
    {
        return await wrappers.Validate<Lesson?, Lesson>(request, async response =>
        {
            var lesson = request.Value;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, lesson.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            lesson.ImageFile.Id = imageFileResponse.Value.Id;

            var guideImageFileResponse = await fileService.SaveImage(new(request.SessionId, lesson.GuideImageFile));
            if (!guideImageFileResponse.Ok)
            {
                response.AddErrors(guideImageFileResponse.Errors);
                return null;
            }

            lesson.GuideImageFile.Id = guideImageFileResponse.Value.Id;

            var guideAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, lesson.GuideAudioFile));
            if (!guideAudioFileResponse.Ok)
            {
                response.AddErrors(guideAudioFileResponse.Errors);
                return null;
            }

            lesson.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await LessonInsert.Execute(connection, transaction, request.SessionId, lesson);

                return new Lesson
                {
                    Id = id,
                    UnitId = lesson.UnitId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Lesson> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var lesson = request.Value;

            var existing = await LessonSelect.Execute(Connection, request.SessionId, lesson);

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, lesson.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            lesson.ImageFile.Id = imageFileResponse.Value.Id;

            var guideImageFileResponse = await fileService.SaveImage(new(request.SessionId, lesson.GuideImageFile), existing?.GuideImageFile);
            if (!guideImageFileResponse.Ok)
            {
                response.AddErrors(guideImageFileResponse.Errors);
                return;
            }

            lesson.GuideImageFile.Id = guideImageFileResponse.Value.Id;

            var guideAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, lesson.GuideAudioFile), existing?.GuideAudioFile);
            if (!guideAudioFileResponse.Ok)
            {
                response.AddErrors(guideAudioFileResponse.Errors);
                return;
            }

            lesson.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await LessonUpdate.Execute(connection, transaction, request.SessionId, lesson);
            });
        });
    }

    public async Task<Response> Remove(Request<Lesson> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var lesson = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await LessonDelete.Execute(connection, transaction, request.SessionId, lesson);
            });
        });
    }

    public async Task<Response<Copy>> Copy(Request<Copy> request)
    {
        return await wrappers.Validate<Copy, Copy>(request, async response =>
        {
            var lessonRequest = new Lesson { Id = request.Value.ExistingId };

            var newLesson = await LessonSelect.Execute(Connection, request.SessionId, lessonRequest);

            if (newLesson is not null)
            {
                newLesson.Id = Guid.NewGuid();
                newLesson.UnitId = request.Value.ExistingParentId;
                newLesson.Title = request.Value.NewName;
                newLesson.GuideAudioFile.OptimizedBlobId = null;
                newLesson.ImageFile.OptimizedBlobId = null;
                newLesson.GuideImageFile.OptimizedBlobId = null;

                if (newLesson.GuideAudioFile.BlobId is not null)
                    newLesson.GuideAudioFile.BlobId = await blobService.Copy(newLesson.GuideAudioFile.BlobId.Value);

                if (newLesson.ImageFile.BlobId is not null)
                    newLesson.ImageFile.BlobId = await blobService.Copy(newLesson.ImageFile.BlobId.Value);

                if (newLesson.GuideImageFile.BlobId is not null)
                    newLesson.GuideImageFile.BlobId = await blobService.Copy(newLesson.GuideImageFile.BlobId.Value);

                newLesson.GuideAudioFile.Id = null;
                newLesson.ImageFile.Id = null;
                newLesson.GuideImageFile.Id = null;

                var newLessonResponse = await Add(new(request.SessionId, newLesson));

                var objectiveRequest = new Request<Lesson>(new() { Id = lessonRequest.Id });
                var objectiveResponse = objectiveService.FetchForLesson(objectiveRequest);

                if (objectiveResponse.Result.Value is not null)
                {
                    foreach (var objective in objectiveResponse.Result.Value.OrderBy(x => x.Ordinal))
                    {
                        if (newLessonResponse.Value is not null)
                        {
                            var objectiveCopy = new Copy
                            {
                                ExistingId = objective.Id,
                                ExistingParentId = newLessonResponse.Value.Id,
                                NewName = objective.Name,
                                NewId = Guid.NewGuid(),
                            };
                            await objectiveService.Copy(new(request.SessionId, objectiveCopy));
                        }
                    }
                }
            }

            return new();
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Lesson>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var lessons = request.Value;

            lessons.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await LessonUpdateOrdinals.Execute(connection, transaction, request.SessionId, lessons);
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
}