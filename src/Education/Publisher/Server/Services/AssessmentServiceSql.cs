namespace Crudspa.Education.Publisher.Server.Services;

public class AssessmentServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IAssessmentService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Assessment>>> Search(Request<AssessmentSearch> request)
    {
        return await wrappers.Try<IList<Assessment>>(request, async response =>
        {
            var assessments = await AssessmentSelectWhere.Execute(Connection, request.SessionId, request.Value);

            return assessments;
        });
    }

    public async Task<Response<Assessment?>> Fetch(Request<Assessment> request)
    {
        return await wrappers.Try<Assessment?>(request, async response =>
        {
            var assessment = await AssessmentSelect.Execute(Connection, request.SessionId, request.Value);

            return assessment;
        });
    }

    public async Task<Response<Assessment?>> Add(Request<Assessment> request)
    {
        return await wrappers.Validate<Assessment?, Assessment>(request, async response =>
        {
            var assessment = request.Value;

            var imageFileFileResponse = await fileService.SaveImage(new(request.SessionId, assessment.ImageFileFile));
            if (!imageFileFileResponse.Ok)
            {
                response.AddErrors(imageFileFileResponse.Errors);
                return null;
            }

            assessment.ImageFileFile.Id = imageFileFileResponse.Value.Id;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await AssessmentInsert.Execute(connection, transaction, request.SessionId, assessment);

                return new Assessment
                {
                    Id = id,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Assessment> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var assessment = request.Value;

            var existing = await AssessmentSelect.Execute(Connection, request.SessionId, assessment);

            var imageFileFileResponse = await fileService.SaveImage(new(request.SessionId, assessment.ImageFileFile), existing?.ImageFileFile);
            if (!imageFileFileResponse.Ok)
            {
                response.AddErrors(imageFileFileResponse.Errors);
                return;
            }

            assessment.ImageFileFile.Id = imageFileFileResponse.Value.Id;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentUpdate.Execute(connection, transaction, request.SessionId, assessment);
            });
        });
    }

    public async Task<Response> Remove(Request<Assessment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var assessment = request.Value;
            var existing = await AssessmentSelect.Execute(Connection, request.SessionId, assessment);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentDelete.Execute(connection, transaction, request.SessionId, assessment);
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

    public async Task<Response<IList<Orderable>>> FetchContentCategoryNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentCategorySelectOrderables.Execute(Connection));
    }
}