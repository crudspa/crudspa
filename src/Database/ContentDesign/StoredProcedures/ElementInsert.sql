create proc [ContentDesign].[ElementInsert] (
     @SessionId uniqueidentifier
    ,@SectionId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@RequireInteraction bit
    ,@BoxId uniqueidentifier
    ,@ItemId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[Element] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SectionId
    ,TypeId
    ,RequireInteraction
    ,BoxId
    ,ItemId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@SectionId
    ,@TypeId
    ,@RequireInteraction
    ,@BoxId
    ,@ItemId
    ,@Ordinal
)

commit transaction