create proc [FrameworkCore].[IconSelectFull] as

select
    icon.Id
    ,icon.Name
    ,icon.CssClass
from [Framework].[Icon-Active] icon
order by icon.Name