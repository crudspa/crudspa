create proc [SamplesCatalog].[ShirtInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(120)
    ,@BrandId uniqueidentifier
    ,@Fit int
    ,@Material nvarchar(80)
    ,@Price real
    ,@HeroImageId uniqueidentifier
    ,@GuidePdfId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Samples].[Shirt] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Name
    ,BrandId
    ,Fit
    ,Material
    ,Price
    ,HeroImageId
    ,GuidePdfId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@BrandId
    ,@Fit
    ,@Material
    ,@Price
    ,@HeroImageId
    ,@GuidePdfId
)

commit transaction