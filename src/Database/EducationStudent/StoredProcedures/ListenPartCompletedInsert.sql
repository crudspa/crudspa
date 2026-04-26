create proc [EducationStudent].[ListenPartCompletedInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ListenPartId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

select top 1
    @Id = completed.Id
from [Education].[ListenPartCompleted] completed with (updlock, holdlock)
where completed.AssignmentId = @AssignmentId
    and completed.ListenPartId = @ListenPartId
    and completed.IsDeleted = 0
order by completed.Updated
    ,completed.Id

if @Id is not null
    return

set @Id = newid()

insert [Education].[ListenPartCompleted] (
     Id
    ,UpdatedBy
    ,AssignmentId
    ,ListenPartId
    ,DeviceTimestamp
)
values (
     @Id
    ,@SessionId
    ,@AssignmentId
    ,@ListenPartId
    ,@DeviceTimestamp
)