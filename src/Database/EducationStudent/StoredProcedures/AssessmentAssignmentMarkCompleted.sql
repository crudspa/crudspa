create proc [EducationStudent].[AssessmentAssignmentMarkCompleted] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[AssessmentAssignment]
set
    Updated = @now
    ,UpdatedBy = @SessionId
    ,Completed = @now
where Id = @Id