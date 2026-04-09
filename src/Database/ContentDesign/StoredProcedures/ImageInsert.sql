create proc [ContentDesign].[ImageInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@FileId uniqueidentifier
    ,@HyperlinkUrl nvarchar(max)
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[ImageElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,FileId
    ,HyperlinkUrl
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@FileId
    ,@HyperlinkUrl
)

commit transaction