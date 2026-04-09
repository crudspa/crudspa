create proc [SamplesCatalog].[ShirtOptionUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@SkuBase nvarchar(40)
    ,@Price real
    ,@ColorId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @existingColorId uniqueidentifier = (select ColorId from [Samples].[ShirtOption] where Id = @Id)

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,SkuBase = @SkuBase
    ,Price = @Price
from [Samples].[ShirtOption] baseTable
    inner join [Samples].[ShirtOption-Active] shirtOption on shirtOption.Id = baseTable.Id
where baseTable.Id = @Id

if (@existingColorId != @ColorId)
begin

    update baseTable
    set
         Id = @Id
        ,Updated = @now
        ,UpdatedBy = @SessionId
        ,ColorId = @ColorId
        ,AllSizes = 1
    from [Samples].[ShirtOption] baseTable
        inner join [Samples].[ShirtOption-Active] shirtOption on shirtOption.Id = baseTable.Id
    where baseTable.Id = @Id

    update [Samples].[ShirtOptionSize]
    set IsDeleted = 1
         ,Updated = @now
         ,UpdatedBy = @SessionId
    where ShirtOptionId = @Id
         and IsDeleted = 0
         and VersionOf = Id
end

commit transaction