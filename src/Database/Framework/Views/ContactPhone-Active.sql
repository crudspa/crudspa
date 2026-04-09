create view [Framework].[ContactPhone-Active] as

select contactPhone.Id as Id
    ,contactPhone.ContactId as ContactId
    ,contactPhone.Phone as Phone
    ,contactPhone.Extension as Extension
    ,contactPhone.SupportsSms as SupportsSms
    ,contactPhone.Ordinal as Ordinal
from [Framework].[ContactPhone] contactPhone
where 1=1
    and contactPhone.IsDeleted = 0
    and contactPhone.VersionOf = contactPhone.Id