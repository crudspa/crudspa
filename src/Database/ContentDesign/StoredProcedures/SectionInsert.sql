create proc [ContentDesign].[SectionInsert] (
     @SessionId uniqueidentifier
    ,@PageId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@BoxId uniqueidentifier
    ,@ContainerId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Content].[Section-Active] where PageId = @PageId)

insert [Content].[Section] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PageId
    ,TypeId
    ,BoxId
    ,ContainerId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PageId
    ,@TypeId
    ,@BoxId
    ,@ContainerId
    ,@ordinal
)

commit transaction