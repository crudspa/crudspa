create view [Education].[Publisher-Active] as

select publisher.Id as Id
    ,publisher.OrganizationId as OrganizationId
    ,publisher.ProviderId as ProviderId
from [Education].[Publisher] publisher
where 1=1
    and publisher.IsDeleted = 0
    and publisher.VersionOf = publisher.Id