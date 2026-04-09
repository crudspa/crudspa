create proc [ContentDesign].[AudioInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@FileId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[AudioElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,FileId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@FileId
)

commit transaction