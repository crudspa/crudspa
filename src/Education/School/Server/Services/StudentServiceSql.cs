using Crudspa.Education.School.Shared.Extensions;

namespace Crudspa.Education.School.Server.Services;

public class StudentServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    ISecretCodeService secretCodeService)
    : IStudentService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Student>>> Search(Request<StudentSearch> request)
    {
        return await wrappers.Try<IList<Student>>(request, async response =>
            await StudentSelectWhere.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<Student?>> Fetch(Request<Student> request)
    {
        return await wrappers.Try<Student?>(request, async response =>
            await StudentSelect.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<Student?>> Add(Request<Student> request)
    {
        return await wrappers.Validate<Student?, Student>(request, async response =>
        {
            var student = request.Value;

            if (!await StudentSecretCodeIsAvailable.Execute(Connection, Guid.Empty, student.SecretCode!))
            {
                response.AddError($"The secret code '{student.SecretCode}' is already being used by another student.");
                return null;
            }

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await StudentInsert.Execute(connection, transaction, request.SessionId, student);
                return new Student { Id = id };
            });
        });
    }

    public async Task<Response> Save(Request<Student> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var student = request.Value;

            if (!await StudentSecretCodeIsAvailable.Execute(Connection, student.Id, student.SecretCode!))
            {
                response.AddError($"The secret code '{student.SecretCode}' is already being used by another student.");
                return;
            }

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StudentUpdate.Execute(connection, transaction, request.SessionId, student);
            });
        });
    }

    public async Task<Response> Remove(Request<Student> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await StudentDelete.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchGrades(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await GradeSelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchAssessmentTypes(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await AssessmentTypeSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchAssessmentLevels(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
        {
            var levels = await AssessmentLevelSelectNames.Execute(Connection);

            foreach (var level in levels)
                level.Name = level.Id.ToFriendlyName();

            return levels;
        });
    }

    public async Task<Response<IList<Named>>> FetchSchoolYears(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await SchoolYearSelectNames.Execute(Connection));
    }

    public async Task<Response<Student>> GenerateSecretCode(Request request)
    {
        return await wrappers.Try<Student>(request, async response =>
            new() { SecretCode = await secretCodeService.Generate() });
    }

    public async Task<Response<District?>> FetchDistrict(Request request)
    {
        return await wrappers.Try<District?>(request, async response =>
            await DistrictSelectBySession.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchSchools(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
        {
            var districtId = (await DistrictSelectBySession.Execute(Connection, request.SessionId))!.Id;
            return await SchoolSelectNamesForDistrict.Execute(Connection, districtId);
        });
    }

    public async Task<Response<IList<Selectable>>> FetchSelectableClassrooms(Request<StudentClassroomSearch> request)
    {
        return await wrappers.Try<IList<Selectable>>(request, async response =>
        {
            var search = request.Value;
            return await StudentSelectableClassrooms.Execute(Connection, request.SessionId, search.StudentId, search.SchoolYearId);
        });
    }
}