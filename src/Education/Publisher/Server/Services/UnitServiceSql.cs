using AchievementSelectNames = Crudspa.Education.Publisher.Server.Sproxies.AchievementSelectNames;

namespace Crudspa.Education.Publisher.Server.Services;

public class UnitServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    ILessonService lessonService,
    IBlobService blobService,
    IUnitBookService unitBookService)
    : IUnitService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Unit>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<Unit>>(request, async response =>
        {
            var units = await UnitSelectAll.Execute(Connection, request.SessionId);

            return units;
        });
    }

    public async Task<Response<Unit?>> Fetch(Request<Unit> request)
    {
        return await wrappers.Try<Unit?>(request, async response =>
        {
            var unit = await UnitSelect.Execute(Connection, request.SessionId, request.Value);

            return unit;
        });
    }

    public async Task<Response<Unit?>> Add(Request<Unit> request)
    {
        return await wrappers.Validate<Unit?, Unit>(request, async response =>
        {
            var unit = request.Value;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, unit.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            unit.ImageFile.Id = imageFileResponse.Value.Id;

            var guideAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, unit.GuideAudioFile));
            if (!guideAudioFileResponse.Ok)
            {
                response.AddErrors(guideAudioFileResponse.Errors);
                return null;
            }

            unit.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

            var introAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, unit.IntroAudioFile));
            if (!introAudioFileResponse.Ok)
            {
                response.AddErrors(introAudioFileResponse.Errors);
                return null;
            }

            unit.IntroAudioFile.Id = introAudioFileResponse.Value.Id;

            var songAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, unit.SongAudioFile));
            if (!songAudioFileResponse.Ok)
            {
                response.AddErrors(songAudioFileResponse.Errors);
                return null;
            }

            unit.SongAudioFile.Id = songAudioFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await UnitInsert.Execute(connection, transaction, request.SessionId, unit);

                return new Unit
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Unit> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var unit = request.Value;

            var existing = await UnitSelect.Execute(Connection, request.SessionId, unit);

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, unit.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            unit.ImageFile.Id = imageFileResponse.Value.Id;

            var guideAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, unit.GuideAudioFile), existing?.GuideAudioFile);
            if (!guideAudioFileResponse.Ok)
            {
                response.AddErrors(guideAudioFileResponse.Errors);
                return;
            }

            unit.GuideAudioFile.Id = guideAudioFileResponse.Value.Id;

            var introAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, unit.IntroAudioFile), existing?.IntroAudioFile);
            if (!introAudioFileResponse.Ok)
            {
                response.AddErrors(introAudioFileResponse.Errors);
                return;
            }

            unit.IntroAudioFile.Id = introAudioFileResponse.Value.Id;

            var songAudioFileResponse = await fileService.SaveAudio(new(request.SessionId, unit.SongAudioFile), existing?.SongAudioFile);
            if (!songAudioFileResponse.Ok)
            {
                response.AddErrors(songAudioFileResponse.Errors);
                return;
            }

            unit.SongAudioFile.Id = songAudioFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitUpdate.Execute(connection, transaction, request.SessionId, unit);
            });
        });
    }

    public async Task<Response<Copy>> Copy(Request<Copy> request)
    {
        return await wrappers.Validate<Copy, Copy>(request, async response =>
        {
            var unitRequest = new Unit { Id = request.Value.ExistingId };

            var newUnit = await UnitSelect.Execute(Connection, request.SessionId, unitRequest);

            if (newUnit is not null)
            {
                newUnit.Id = Guid.NewGuid();
                newUnit.Title = request.Value.NewName;
                newUnit.GuideAudioFile.OptimizedBlobId = null;
                newUnit.ImageFile.OptimizedBlobId = null;
                newUnit.IntroAudioFile.OptimizedBlobId = null;
                newUnit.SongAudioFile.OptimizedBlobId = null;

                if (request.Value.ExistingParentId.HasValue)
                    newUnit.ParentId = request.Value.ExistingParentId;

                if (newUnit.GuideAudioFile.BlobId is not null)
                    newUnit.GuideAudioFile.BlobId = await blobService.Copy(newUnit.GuideAudioFile.BlobId.Value);
                if (newUnit.ImageFile.BlobId is not null)
                    newUnit.ImageFile.BlobId = await blobService.Copy(newUnit.ImageFile.BlobId.Value);
                if (newUnit.IntroAudioFile.BlobId is not null)
                    newUnit.IntroAudioFile.BlobId = await blobService.Copy(newUnit.IntroAudioFile.BlobId.Value);
                if (newUnit.SongAudioFile.BlobId is not null)
                    newUnit.SongAudioFile.BlobId = await blobService.Copy(newUnit.SongAudioFile.BlobId.Value);

                newUnit.GuideAudioFile.Id = null;
                newUnit.ImageFile.Id = null;
                newUnit.IntroAudioFile.Id = null;
                newUnit.SongAudioFile.Id = null;

                var newUnitResponse = await Add(new(request.SessionId, newUnit));

                var lessonRequest = new Request<Unit>(new() { Id = unitRequest.Id });
                var lessonResponse = lessonService.FetchForUnit(lessonRequest);

                if (lessonResponse.Result.Value is not null)
                {
                    foreach (var lesson in lessonResponse.Result.Value.OrderBy(x => x.Ordinal))
                    {
                        if (newUnitResponse.Value is not null)
                        {
                            request.Value.NewId = newUnitResponse.Value.Id;

                            var lessonCopy = new Copy
                            {
                                ExistingId = lesson.Id,
                                ExistingParentId = newUnitResponse.Value.Id,
                                NewName = lesson.Name,
                                NewId = Guid.NewGuid(),
                            };
                            await lessonService.Copy(new(request.SessionId, lessonCopy));
                        }
                    }
                }

                var unitBookResponse = unitBookService.FetchForUnit(lessonRequest);

                if (unitBookResponse.Result.Value is not null)
                {
                    foreach (var unitBook in unitBookResponse.Result.Value.OrderBy(x => x.Ordinal))
                    {
                        if (newUnitResponse.Value is not null)
                        {
                            var unitBookCopy = new Copy
                            {
                                ExistingId = unitBook.Id,
                                ExistingParentId = newUnitResponse.Value.Id,
                                NewName = unitBook.BookTitle,
                                NewId = Guid.NewGuid(),
                            };
                            await unitBookService.Copy(new(request.SessionId, unitBookCopy));
                        }
                    }
                }

                var childUnits = await UnitSelectChildren.Execute(Connection, unitRequest);
                if (newUnitResponse.Value is not null)
                {
                    foreach (var child in childUnits)
                    {
                        var childCopy = new Copy
                        {
                            ExistingId = child.Id,
                            ExistingParentId = newUnitResponse.Value.Id,
                            NewName = child.Title + " (Copy)",
                            NewId = Guid.NewGuid(),
                        };
                        await Copy(new(request.SessionId, childCopy));
                    }
                }
            }

            return request.Value;
        });
    }

    public async Task<Response> Remove(Request<Unit> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var unit = request.Value;
            var existing = await UnitSelect.Execute(Connection, request.SessionId, unit);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitDelete.Execute(connection, transaction, request.SessionId, unit);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<Unit>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var units = request.Value;

            units.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await UnitUpdateOrdinals.Execute(connection, transaction, request.SessionId, units);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GradeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchUnitNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await UnitSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchAchievementNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AchievementSelectNames.Execute(Connection));
    }
}