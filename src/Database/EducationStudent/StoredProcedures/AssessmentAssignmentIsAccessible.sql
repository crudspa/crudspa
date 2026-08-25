create proc [EducationStudent].[AssessmentAssignmentIsAccessible] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select cast(case when exists (
    select 1
    from [Education].[AssessmentAssignment-Active] assessmentAssignment
        inner join [EducationStudent].[LicensedAssessments](@SessionId) licensedAssessment
            on licensedAssessment.AssessmentId = assessmentAssignment.AssessmentId
        inner join [Education].[Student-Active] student on student.Id = assessmentAssignment.StudentId
        inner join [Framework].[User-Active] userTable on userTable.ContactId = student.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where assessmentAssignment.Id = @Id
        and session.Id = @SessionId
        and session.Ended is null
        and assessmentAssignment.Terminated is null
) then 1 else 0 end as int)