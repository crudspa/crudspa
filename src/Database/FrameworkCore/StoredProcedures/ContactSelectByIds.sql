create proc [FrameworkCore].[ContactSelectByIds] (
     @Ids Framework.IdList readonly
) as

set nocount on

select
     contact.Id
    ,contact.FirstName
    ,contact.LastName
    ,contact.TimeZoneId
from [Framework].[Contact-Active] contact
    inner join @Ids ids on ids.Id = contact.Id

select
     contactEmail.Id
    ,contactEmail.ContactId
    ,contactEmail.Email
    ,contactEmail.Ordinal
from [Framework].[ContactEmail-Active] contactEmail
    inner join @Ids ids on ids.Id = contactEmail.ContactId

select
     contactPhone.Id
    ,contactPhone.ContactId
    ,contactPhone.Phone
    ,contactPhone.Extension
    ,contactPhone.SupportsSms
    ,contactPhone.Ordinal
from [Framework].[ContactPhone-Active] contactPhone
    inner join @Ids ids on ids.Id = contactPhone.ContactId

select
     contactPostal.Id
    ,contactPostal.ContactId
    ,contactPostal.PostalId
    ,usaPostal.RecipientName as PostalRecipientName
    ,usaPostal.BusinessName as PostalBusinessName
    ,usaPostal.StreetAddress as PostalStreetAddress
    ,usaPostal.City as PostalCity
    ,usaPostal.StateId as PostalStateId
    ,usaPostal.PostalCode as PostalPostalCode
    ,contactPostal.Ordinal
from [Framework].[ContactPostal-Active] contactPostal
    inner join @Ids ids on ids.Id = contactPostal.ContactId
    inner join [Framework].[UsaPostal-Active] usaPostal on contactPostal.PostalId = usaPostal.Id