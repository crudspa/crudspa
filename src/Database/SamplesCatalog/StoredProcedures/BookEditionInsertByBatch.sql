create proc [SamplesCatalog].[BookEditionInsertByBatch] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@FormatId uniqueidentifier
    ,@Sku nvarchar(40)
    ,@Price real
    ,@ReleasedOn date
    ,@InPrint bit
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Samples].[BookEdition] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BookId
    ,FormatId
    ,Sku
    ,Price
    ,ReleasedOn
    ,InPrint
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@BookId
    ,@FormatId
    ,@Sku
    ,@Price
    ,@ReleasedOn
    ,@InPrint
    ,@Ordinal
)

commit transaction