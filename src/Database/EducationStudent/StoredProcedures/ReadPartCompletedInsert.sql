create proc [EducationStudent].[ReadPartCompletedInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ReadPartId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

select top 1
    @Id = completed.Id
from [Education].[ReadPartCompleted] completed with (updlock, holdlock)
where completed.AssignmentId = @AssignmentId
    and completed.ReadPartId = @ReadPartId
    and completed.IsDeleted = 0
order by completed.Updated
    ,completed.Id

if @Id is not null
    return

set @Id = newid()

insert [Education].[ReadPartCompleted] (
     Id
    ,UpdatedBy
    ,AssignmentId
    ,ReadPartId
    ,DeviceTimestamp
)
values (
     @Id
    ,@SessionId
    ,@AssignmentId
    ,@ReadPartId
    ,@DeviceTimestamp
)