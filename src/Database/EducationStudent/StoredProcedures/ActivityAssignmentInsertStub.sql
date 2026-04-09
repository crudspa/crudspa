create proc [EducationStudent].[ActivityAssignmentInsertStub] (
     @SessionId uniqueidentifier
    ,@AssignmentBatchId uniqueidentifier
    ,@ActivityId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ActivityAssignment] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,AssignmentBatchId
    ,ActivityId
    ,Ordinal
    ,StatusId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@AssignmentBatchId
    ,@ActivityId
    ,@Ordinal
    ,'ee23e01e-9d01-4b59-861d-17a6aa9aea53' -- Not Started
)