create proc [EducationStudent].[ReadPartCompletedInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@ReadPartId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

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