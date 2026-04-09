create proc [SamplesCatalog].[ShirtOptionSelectForShirt] (
     @SessionId uniqueidentifier
    ,@ShirtId uniqueidentifier
) as

set nocount on

select
     shirtOption.Id
    ,shirtOption.ShirtId
    ,shirtOption.SkuBase
    ,shirtOption.Price
    ,shirtOption.ColorId
    ,color.Name as ColorName
    ,shirtOption.AllSizes
    ,shirtOption.Ordinal
from [Samples].[ShirtOption-Active] shirtOption
    inner join [Samples].[Color-Active] color on shirtOption.ColorId = color.Id
    inner join [Samples].[Shirt-Active] shirt on shirtOption.ShirtId = shirt.Id
where shirtOption.ShirtId = @ShirtId


select distinct
     shirtOption.Id as RootId
    ,sizeTable.Id as Id
    ,sizeTable.Name as Name
    ,convert(bit, case when shirtOption.AllSizes = 1 or shirtOptionSize.Id is not null then 1 else 0 end) as Selected
    ,sizeTable.Ordinal
from [Samples].[ShirtOption-Active] shirtOption
    inner join [Samples].[Color-Active] color on color.Id = shirtOption.ColorId
    inner join [Samples].[Size-Active] sizeTable on sizeTable.ColorId = color.Id
    left join [Samples].[ShirtOptionSize-Active] shirtOptionSize on shirtOptionSize.ShirtOptionId = shirtOption.Id
        and shirtOptionSize.SizeId = sizeTable.Id
where shirtOption.ShirtId = @ShirtId
order by sizeTable.Ordinal