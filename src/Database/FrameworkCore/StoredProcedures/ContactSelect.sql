create proc [FrameworkCore].[ContactSelect] (
     @Id uniqueidentifier
) as

select
     contact.Id
    ,contact.FirstName
    ,contact.LastName
    ,contact.TimeZoneId
from [Framework].[Contact-Active] contact
where Id = @Id

select
     contactEmail.Id
    ,contactEmail.ContactId
    ,contactEmail.Email
    ,contactEmail.Ordinal
from [Framework].[ContactEmail-Active] contactEmail
where contactEmail.ContactId = @Id
order by contactEmail.Ordinal, contactEmail.Email

select
     contactPhone.Id
    ,contactPhone.ContactId
    ,contactPhone.Phone
    ,contactPhone.Extension
    ,contactPhone.SupportsSms
    ,contactPhone.Ordinal
from [Framework].[ContactPhone-Active] contactPhone
where contactPhone.ContactId = @Id
order by contactPhone.Ordinal, contactPhone.Phone

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
    inner join [Framework].[UsaPostal-Active] usaPostal on contactPostal.PostalId = usaPostal.Id
where contactPostal.ContactId = @Id
order by contactPostal.Ordinal, usaPostal.RecipientName, usaPostal.StreetAddress