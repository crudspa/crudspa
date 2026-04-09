create proc [SamplesCatalog].[ShirtOptionInsert] (
     @SessionId uniqueidentifier
    ,@ShirtId uniqueidentifier
    ,@SkuBase nvarchar(40)
    ,@Price real
    ,@ColorId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

declare @ordinal int = (select count(1) from [Samples].[ShirtOption-Active] where ShirtId = @ShirtId)

insert [Samples].[ShirtOption] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ShirtId
    ,SkuBase
    ,Price
    ,ColorId
    ,AllSizes
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ShirtId
    ,@SkuBase
    ,@Price
    ,@ColorId
    ,1
    ,@ordinal
)

commit transaction