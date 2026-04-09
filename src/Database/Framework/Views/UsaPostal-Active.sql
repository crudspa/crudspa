create view [Framework].[UsaPostal-Active] as

select usaPostal.Id as Id
    ,usaPostal.RecipientName as RecipientName
    ,usaPostal.BusinessName as BusinessName
    ,usaPostal.StreetAddress as StreetAddress
    ,usaPostal.City as City
    ,usaPostal.StateId as StateId
    ,usaPostal.PostalCode as PostalCode
from [Framework].[UsaPostal] usaPostal
where 1=1
    and usaPostal.IsDeleted = 0
    and usaPostal.VersionOf = usaPostal.Id