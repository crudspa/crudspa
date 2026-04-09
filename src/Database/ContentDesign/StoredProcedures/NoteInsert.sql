create proc [ContentDesign].[NoteInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@Instructions nvarchar(max)
    ,@ImageFileId uniqueidentifier
    ,@RequireText bit
    ,@RequireImageSelection bit
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[NoteElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,Instructions
    ,ImageFileId
    ,RequireText
    ,RequireImageSelection
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@Instructions
    ,@ImageFileId
    ,@RequireText
    ,@RequireImageSelection
)

commit transaction