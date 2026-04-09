create proc [EducationStudent].[ListenPartCompletedInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ListenPartId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

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