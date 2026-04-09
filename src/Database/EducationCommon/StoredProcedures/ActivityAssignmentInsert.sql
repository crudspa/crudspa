create proc [EducationCommon].[ActivityAssignmentInsert] (
     @SessionId uniqueidentifier
    ,@AssignmentBatchId uniqueidentifier
    ,@ActivityId uniqueidentifier
    ,@Ordinal int
    ,@Started datetimeoffset
    ,@Finished datetimeoffset
    ,@StatusId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    insert [Education].[ActivityAssignment] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,AssignmentBatchId
        ,ActivityId
        ,Ordinal
        ,Started
        ,Finished
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
        ,@Started
        ,@Finished
        ,@StatusId
    )

commit transaction