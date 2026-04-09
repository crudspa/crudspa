namespace Crudspa.Education.School.Server.Services;

public class AssessmentAssignmentServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAssessmentProgressService assessmentProgressService)
    : IAssessmentAssignmentService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<AssessmentAssignment>>> Search(Request<AssessmentAssignmentSearch> request)
    {
        return await wrappers.Try<IList<AssessmentAssignment>>(request, async response =>
        {
            return await AssessmentAssignmentSelectWhere.Execute(Connection, request.Value, request.SessionId);
        });
    }

    public async Task<Response<AssessmentAssignment?>> Fetch(Request<AssessmentAssignment> request)
    {
        return await wrappers.Try<AssessmentAssignment?>(request, async response =>
        {
            var assessmentAssignment = await AssessmentAssignmentSelect.Execute(Connection, request.SessionId, request.Value);
            return assessmentAssignment;
        });
    }

    public async Task<Response<Assessment?>> FetchProgress(Request<AssessmentAssignment> request)
    {
        return await assessmentProgressService.Fetch(request);
    }

    public async Task<Response<AssessmentAssignment?>> Add(Request<AssessmentAssignment> request)
    {
        return await wrappers.Validate<AssessmentAssignment?, AssessmentAssignment>(request, async response =>
        {
            var assessmentAssignment = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                assessmentAssignment.Id = await AssessmentAssignmentInsert.Execute(connection, transaction, request.SessionId, assessmentAssignment);
            });

            return await AssessmentAssignmentSelect.Execute(Connection, request.SessionId, assessmentAssignment);
        });
    }

    public async Task<Response> Save(Request<AssessmentAssignment> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var assessmentAssignment = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentAssignmentUpdate.Execute(connection, transaction, request.SessionId, assessmentAssignment);
            });
        });
    }

    public async Task<Response> Remove(Request<AssessmentAssignment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var assessmentAssignment = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentAssignmentDelete.Execute(connection, transaction, request.SessionId, assessmentAssignment);
            });
        });
    }

    public async Task<Response> ResetProgress(Request<AssessmentAssignment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var assessmentAssignment = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentAssignmentResetProgress.Execute(connection, transaction, request.SessionId, assessmentAssignment);
            });
        });
    }

    public async Task<Response<IList<Named>>> FetchStudentNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await StudentSelectNames.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<Named>>> FetchAssessmentNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await AssessmentSelectNamesCurrent.Execute(Connection));
    }

    public async Task<Response<IList<Named>>> FetchClassroomNames(Request request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await ClassroomSelectNamesBySession.Execute(Connection, request.SessionId));
    }

    public async Task<Response<AssessmentAssignmentBulk>> BulkAssign(Request<AssessmentAssignmentBulk> request)
    {
        return await wrappers.Validate<AssessmentAssignmentBulk, AssessmentAssignmentBulk>(request, async response =>
        {
            var bulk = request.Value;

            var doAdditions = bulk.Action is AssessmentAssignmentBulk.Actions.AddAndUpdate or AssessmentAssignmentBulk.Actions.Add;
            var doUpdates = bulk.Action is AssessmentAssignmentBulk.Actions.AddAndUpdate or AssessmentAssignmentBulk.Actions.Update;

            IList<AssessmentAssignment> existingAssignments;
            IList<Student> allStudents;

            switch (bulk.Scope)
            {
                case AssessmentAssignmentBulk.Scopes.EntireSchool:
                    existingAssignments = await AssessmentAssignmentSelectForSchool.Execute(Connection, request.SessionId, bulk.AssessmentId);
                    allStudents = await StudentSelectForSchool.Execute(Connection, request.SessionId, bulk.AssessmentId);
                    break;
                case AssessmentAssignmentBulk.Scopes.Classroom:
                    existingAssignments = await AssessmentAssignmentSelectForClassroom.Execute(Connection, request.SessionId, bulk.AssessmentId, bulk.ClassroomId);
                    allStudents = await StudentSelectForClassroom.Execute(Connection, request.SessionId, bulk.ClassroomId, bulk.AssessmentId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            var newStudents = allStudents
                .Where(x => existingAssignments.All(y => y.StudentId != x.Id))
                .ToList();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                if (doUpdates)
                {
                    foreach (var existing in existingAssignments)
                    {
                        var updatedAssignment = new AssessmentAssignment
                        {
                            Id = existing.Id,
                            AssessmentId = existing.AssessmentId,
                            StudentId = existing.StudentId,
                            StartAfter = bulk.StartAfter,
                            EndBefore = bulk.EndBefore,
                        };
                        await AssessmentAssignmentUpdate.Execute(connection, transaction, request.SessionId, updatedAssignment);
                    }

                    bulk.RecordsUpdated = existingAssignments.Count;
                }

                if (doAdditions)
                {
                    foreach (var student in newStudents)
                    {
                        var newAssignment = new AssessmentAssignment
                        {
                            AssessmentId = bulk.AssessmentId,
                            StudentId = student.Id,
                            StartAfter = bulk.StartAfter,
                            EndBefore = bulk.EndBefore,
                        };
                        await AssessmentAssignmentInsert.Execute(connection, transaction, request.SessionId, newAssignment);
                    }

                    bulk.RecordsAdded = newStudents.Count;
                }
            });

            return bulk;
        });
    }
}