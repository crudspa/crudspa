create proc [EducationStudent].[AssessmentAssignmentMarkCompleted] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update assessmentAssignment
set
    assessmentAssignment.Updated = @now
    ,UpdatedBy = @SessionId
    ,Completed = @now
from [Education].[AssessmentAssignment] assessmentAssignment
    inner join [Education].[AssessmentAssignment-Active] activeAssignment on activeAssignment.Id = assessmentAssignment.Id
    inner join [EducationStudent].[LicensedAssessments](@SessionId) licensedAssessment on licensedAssessment.AssessmentId = activeAssignment.AssessmentId
    inner join [Education].[Student-Active] student on student.Id = activeAssignment.StudentId
    inner join [Framework].[User-Active] userTable on userTable.ContactId = student.ContactId
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
where assessmentAssignment.Id = @Id
    and session.Id = @SessionId
    and session.Ended is null
    and assessmentAssignment.Completed is null