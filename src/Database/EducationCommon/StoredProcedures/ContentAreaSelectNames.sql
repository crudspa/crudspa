create proc [EducationCommon].[ContentAreaSelectNames] as

select
    contentArea.Id,
    case when contentArea.Name = contentArea.AppNavText then contentArea.Name
         else contentArea.Name + ' (' + contentArea.AppNavText + ')'
    end as Name
from [Education].[ContentArea-Active] contentArea
order by contentArea.Name, contentArea.AppNavText