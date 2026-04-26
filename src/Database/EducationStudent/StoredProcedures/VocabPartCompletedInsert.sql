create proc [EducationStudent].[VocabPartCompletedInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@VocabPartId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

select top 1
    @Id = completed.Id
from [Education].[VocabPartCompleted] completed with (updlock, holdlock)
where completed.AssignmentId = @AssignmentId
    and completed.VocabPartId = @VocabPartId
    and completed.IsDeleted = 0
order by completed.Updated
    ,completed.Id

if @Id is not null
    return

set @Id = newid()

insert [Education].[VocabPartCompleted] (
     Id
    ,UpdatedBy
    ,AssignmentId
    ,VocabPartId
    ,DeviceTimestamp
)
values (
     @Id
    ,@SessionId
    ,@AssignmentId
    ,@VocabPartId
    ,@DeviceTimestamp
)