create proc [EducationStudent].[AssignmentBatchInsert] (
     @SessionId uniqueidentifier
    ,@GameId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[AssignmentBatch] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,GameId
    ,StudentId
    ,Published
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@GameId
    ,@StudentId
    ,sysdatetimeoffset()
)