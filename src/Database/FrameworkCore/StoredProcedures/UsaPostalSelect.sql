create proc [FrameworkCore].[UsaPostalSelect] (
     @Id uniqueidentifier
) as

select
     usaPostal.Id
    ,usaPostal.RecipientName
    ,usaPostal.BusinessName
    ,usaPostal.StreetAddress
    ,usaPostal.City
    ,usaPostal.StateId
    ,usaPostal.PostalCode
from [Framework].[UsaPostal-Active] usaPostal
where usaPostal.Id = @Id