namespace Crudspa.Education.School.Server.Services;

public class ClassroomServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService)
    : IClassroomService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Classroom>>> FetchAll(Request request)
    {
        return await wrappers.Try<IList<Classroom>>(request, async response =>
            await ClassroomSelectAll.Execute(Connection, request.SessionId));
    }

    public async Task<Response<Classroom?>> Fetch(Request<Classroom> request)
    {
        return await wrappers.Try<Classroom?>(request, async response =>
        {
            var classroom = await ClassroomSelect.Execute(Connection, request.Value);

            if (classroom is null)
                return null;

            classroom.ClassroomStudents = (await ClassroomStudentSelectForClassroom.Execute(Connection, classroom.Id)).ToObservable();
            classroom.ClassroomTeachers = (await ClassroomTeacherSelectForClassroom.Execute(Connection, classroom.Id)).ToObservable();

            return classroom;
        });
    }

    public async Task<Response<Classroom?>> Add(Request<Classroom> request)
    {
        return await wrappers.Validate<Classroom?, Classroom>(request, async response =>
        {
            var classroom = request.Value;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                classroom.Id = await ClassroomInsert.Execute(connection, transaction, request.SessionId, classroom);
                foreach (var classroomStudent in classroom.ClassroomStudents)
                {
                    classroomStudent.ClassroomId = classroom.Id;
                    await ClassroomStudentInsert.Execute(connection, transaction, request.SessionId, classroomStudent);
                }

                foreach (var classroomTeacher in classroom.ClassroomTeachers)
                {
                    classroomTeacher.ClassroomId = classroom.Id;
                    await ClassroomTeacherInsert.Execute(connection, transaction, request.SessionId, classroomTeacher);
                }
            });

            return await ClassroomSelect.Execute(Connection, classroom);
        });
    }

    public async Task<Response> Save(Request<Classroom> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var classroom = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ClassroomUpdate.Execute(connection, transaction, request.SessionId, classroom);

                var existingClassroomStudents = await ClassroomStudentSelectForClassroom.Execute(Connection, classroom.Id);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existingClassroomStudents,
                    classroom.ClassroomStudents,
                    ClassroomStudentInsert.Execute,
                    ClassroomStudentUpdate.Execute,
                    ClassroomStudentDelete.Execute);

                var existingClassroomTeachers = await ClassroomTeacherSelectForClassroom.Execute(Connection, classroom.Id);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existingClassroomTeachers,
                    classroom.ClassroomTeachers,
                    ClassroomTeacherInsert.Execute,
                    ClassroomTeacherUpdate.Execute,
                    ClassroomTeacherDelete.Execute);
            });
        });
    }

    public async Task<Response> Remove(Request<Classroom> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var classroom = request.Value;
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ClassroomDelete.Execute(connection, transaction, request.SessionId, classroom);
            });
        });
    }

    public async Task<Response<IList<Selectable>>> FetchStudents(Request<Classroom> request)
    {
        return await wrappers.Try<IList<Selectable>>(request, async response =>
            await ClassroomSelectableStudents.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Selectable>>> FetchSchoolContacts(Request<Classroom> request)
    {
        return await wrappers.Try<IList<Selectable>>(request, async response =>
            await ClassroomSelectableSchoolContacts.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Orderable>>> FetchTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ClassroomTypeSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchGradeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ClassroomGradeSelectNames.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchClassroomNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ClassroomSelectNamesBySession.Execute(Connection, request.SessionId));
    }

    public async Task<Response<District?>> FetchDistrict(Request request)
    {
        return await wrappers.Try<District?>(request, async response =>
            await DistrictSelectBySession.Execute(Connection, request.SessionId));
    }
}