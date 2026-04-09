create proc [SamplesCatalog].[BookEditionUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@BookId uniqueidentifier
    ,@FormatId uniqueidentifier
    ,@Sku nvarchar(40)
    ,@Price real
    ,@ReleasedOn date
    ,@InPrint bit
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Samples].[BookEdition]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,FormatId = @FormatId
    ,Sku = @Sku
    ,Price = @Price
    ,ReleasedOn = @ReleasedOn
    ,InPrint = @InPrint
    ,Ordinal = @Ordinal
where Id = @Id

commit transaction