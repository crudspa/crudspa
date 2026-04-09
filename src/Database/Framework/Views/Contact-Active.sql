create view [Framework].[Contact-Active] as

select contact.Id as Id
    ,contact.FirstName as FirstName
    ,contact.LastName as LastName
    ,contact.TimeZoneId as TimeZoneId
from [Framework].[Contact] contact
where 1=1
    and contact.IsDeleted = 0
    and contact.VersionOf = contact.Id