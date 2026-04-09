create proc [ContentDisplay].[PageSelectForBinder] (
     @BinderId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

select
     page.Id as Id
    ,page.BinderId as BinderId
    ,page.Title as Title
    ,page.GuideText as GuideText
    ,page.Ordinal as Ordinal
    ,status.Name as StatusName
    ,(select count(1) from [Content].[Section-Active] section where section.PageId = page.Id) as SectionCount
from [Content].[Page-Active] page
    inner join [Framework].[ContentStatus-Active] status on page.StatusId = status.Id
where page.BinderId = @BinderId
    and page.StatusId = @ContentStatusComplete
order by page.Ordinal