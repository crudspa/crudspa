create view [Education].[Report-Active] as

select report.Id as Id
    ,report.PortalId as PortalId
    ,report.IconId as IconId
    ,report.DisplayView as DisplayView
    ,report.Name as Name
    ,report.Description as Description
    ,report.PermissionId as PermissionId
    ,report.Ordinal as Ordinal
from [Education].[Report] report
where 1=1
    and report.IsDeleted = 0