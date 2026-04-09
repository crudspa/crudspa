create trigger [Framework].[UsaPostalTrigger] on [Framework].[UsaPostal]
    for update
as

insert [Framework].[UsaPostal] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,RecipientName
    ,BusinessName
    ,StreetAddress
    ,City
    ,StateId
    ,PostalCode
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.RecipientName
    ,deleted.BusinessName
    ,deleted.StreetAddress
    ,deleted.City
    ,deleted.StateId
    ,deleted.PostalCode
from deleted