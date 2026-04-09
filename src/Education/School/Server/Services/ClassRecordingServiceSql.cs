namespace Crudspa.Education.School.Server.Services;

public class ClassRecordingServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IClassRecordingService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ClassRecording>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<ClassRecording>>(request, async response =>
        {
            var classRecordings = await ClassRecordingSelectAll.Execute(Connection, request.SessionId);
            return classRecordings;
        });
    }

    public async Task<Response<ClassRecording?>> Fetch(Request<ClassRecording> request)
    {
        return await wrappers.Try<ClassRecording?>(request, async response =>
        {
            var classRecording = await ClassRecordingSelect.Execute(Connection, request.SessionId, request.Value);
            return classRecording;
        });
    }

    public async Task<Response<ClassRecording?>> Add(Request<ClassRecording> request)
    {
        return await wrappers.Validate<ClassRecording?, ClassRecording>(request, async response =>
        {
            var classRecording = request.Value;

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, classRecording.AudioFileFile));
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return null;
            }

            classRecording.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, classRecording.ImageFile));
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return null;
            }

            classRecording.ImageFile.Id = imageFileResponse.Value.Id;

            classRecording.TeacherNotes = htmlSanitizer.Sanitize(classRecording.TeacherNotes);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ClassRecordingInsert.Execute(connection, transaction, request.SessionId, classRecording);

                return new ClassRecording
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ClassRecording> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var classRecording = request.Value;

            var existing = await ClassRecordingSelect.Execute(Connection, request.SessionId, classRecording);

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, classRecording.AudioFileFile), existing?.AudioFileFile);
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return;
            }

            classRecording.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            var imageFileResponse = await fileService.SaveImage(new(request.SessionId, classRecording.ImageFile), existing?.ImageFile);
            if (!imageFileResponse.Ok)
            {
                response.AddErrors(imageFileResponse.Errors);
                return;
            }

            classRecording.ImageFile.Id = imageFileResponse.Value.Id;

            classRecording.TeacherNotes = htmlSanitizer.Sanitize(classRecording.TeacherNotes);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ClassRecordingUpdate.Execute(connection, transaction, request.SessionId, classRecording);
            });
        });
    }

    public async Task<Response> Remove(Request<ClassRecording> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var classRecording = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ClassRecordingDelete.Execute(connection, transaction, request.SessionId, classRecording);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentCategoryNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentCategorySelectOrderables.Execute(Connection));
    }
}