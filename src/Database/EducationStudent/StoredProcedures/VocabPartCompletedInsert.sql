create proc [EducationStudent].[VocabPartCompletedInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentId uniqueidentifier
    ,@VocabPartId uniqueidentifier
    ,@DeviceTimestamp datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

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