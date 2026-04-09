create proc [SamplesCatalog].[ShirtUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(120)
    ,@BrandId uniqueidentifier
    ,@Fit int
    ,@Material nvarchar(80)
    ,@Price real
    ,@HeroImageId uniqueidentifier
    ,@GuidePdfId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,BrandId = @BrandId
    ,Fit = @Fit
    ,Material = @Material
    ,Price = @Price
    ,HeroImageId = @HeroImageId
    ,GuidePdfId = @GuidePdfId
from [Samples].[Shirt] baseTable
    inner join [Samples].[Shirt-Active] shirt on shirt.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction