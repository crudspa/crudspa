create proc [EducationStudent].[AssessmentAssignmentMarkStarted] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[AssessmentAssignment]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,Started = @now
where Id = @Id
    and Started is null