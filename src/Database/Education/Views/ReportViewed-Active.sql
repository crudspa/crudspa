create view [Education].[ReportViewed-Active] as

select reportViewed.Id as Id
    ,reportViewed.ReportId as ReportId
from [Education].[ReportViewed] reportViewed
where 1=1
    and reportViewed.IsDeleted = 0