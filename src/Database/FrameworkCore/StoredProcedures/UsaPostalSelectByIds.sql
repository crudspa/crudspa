create proc [FrameworkCore].[UsaPostalSelectByIds] (
     @Ids Framework.IdList readonly
) as

set nocount on

select
     usaPostal.Id
    ,usaPostal.RecipientName
    ,usaPostal.BusinessName
    ,usaPostal.StreetAddress
    ,usaPostal.City
    ,usaPostal.StateId
    ,usaPostal.PostalCode
from [Framework].[UsaPostal-Active] usaPostal
    inner join @Ids ids on ids.Id = usaPostal.Id