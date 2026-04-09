create view [Framework].[ContactPostal-Active] as

select contactPostal.Id as Id
    ,contactPostal.ContactId as ContactId
    ,contactPostal.PostalId as PostalId
    ,contactPostal.Ordinal as Ordinal
from [Framework].[ContactPostal] contactPostal
where 1=1
    and contactPostal.IsDeleted = 0
    and contactPostal.VersionOf = contactPostal.Id