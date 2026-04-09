create proc [ContentDesign].[MultimediaElementInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@ContainerId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[MultimediaElement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ElementId
    ,ContainerId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ElementId
    ,@ContainerId
)

commit transaction