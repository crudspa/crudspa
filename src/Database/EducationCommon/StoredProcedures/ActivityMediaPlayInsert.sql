create proc [EducationCommon].[ActivityMediaPlayInsert] (
     @SessionId uniqueidentifier
    ,@AudioFileId uniqueidentifier
    ,@VideoFileId uniqueidentifier
    ,@Started datetimeoffset
    ,@Canceled datetimeoffset
    ,@Completed datetimeoffset
    ,@AssignmentBatchId uniqueidentifier
    ,@ActivityId uniqueidentifier
    ,@ActivityChoiceId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

declare @mediaPlayId uniqueidentifier = newid()

begin transaction

    insert [Framework].[MediaPlay] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,AudioFileId
        ,VideoFileId
        ,Started
        ,Canceled
        ,Completed
    )
    values (
        @mediaPlayId
        ,@mediaPlayId
        ,@now
        ,@SessionId
        ,@AudioFileId
        ,@VideoFileId
        ,isnull(@Started, sysdatetimeoffset())
        ,@Canceled
        ,@Completed
    )

    insert [Education].[ActivityMediaPlay] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,MediaPlayId
        ,AssignmentBatchId
        ,ActivityId
        ,ActivityChoiceId
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@mediaPlayId
        ,@AssignmentBatchId
        ,@ActivityId
        ,@ActivityChoiceId
    )
commit transaction