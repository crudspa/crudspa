create view [Content].[PageType-Active] as

select pageType.Id as Id
    ,pageType.Name as Name
    ,pageType.DesignView as DesignView
    ,pageType.DisplayView as DisplayView
    ,pageType.Ordinal as Ordinal
from [Content].[PageType] pageType
where 1=1
    and pageType.IsDeleted = 0