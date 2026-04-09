create proc [ContentDesign].[VideoInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@FileId uniqueidentifier
    ,@AutoPlay bit
    ,@PosterId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[VideoElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,FileId
    ,AutoPlay
    ,PosterId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@FileId
    ,@AutoPlay
    ,@PosterId
)

commit transaction