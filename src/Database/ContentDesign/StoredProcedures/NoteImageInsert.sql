create proc [ContentDesign].[NoteImageInsert] (
     @SessionId uniqueidentifier
    ,@NoteId uniqueidentifier
    ,@ImageFileId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[NoteImage] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,NoteId
    ,ImageFileId
    ,Ordinal
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@NoteId
    ,@ImageFileId
    ,@Ordinal
)