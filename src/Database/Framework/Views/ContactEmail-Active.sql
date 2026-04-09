create view [Framework].[ContactEmail-Active] as

select contactEmail.Id as Id
    ,contactEmail.ContactId as ContactId
    ,contactEmail.Email as Email
    ,contactEmail.Ordinal as Ordinal
from [Framework].[ContactEmail] contactEmail
where 1=1
    and contactEmail.IsDeleted = 0
    and contactEmail.VersionOf = contactEmail.Id