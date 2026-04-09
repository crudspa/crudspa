create proc [FrameworkCore].[MediaPlayInsert] (
     @SessionId uniqueidentifier
    ,@AudioFileId uniqueidentifier
    ,@VideoFileId uniqueidentifier
    ,@Started datetimeoffset
    ,@Canceled datetimeoffset
    ,@Completed datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

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
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@AudioFileId
    ,@VideoFileId
    ,isnull(@Started, sysdatetimeoffset())
    ,@Canceled
    ,@Completed
)