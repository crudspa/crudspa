create proc [SamplesCatalog].[CatalogContactSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

set nocount on

select
     catalogContact.Id
    ,catalogContact.ContactId
    ,catalogContact.UserId
from [Samples].[CatalogContact-Active] catalogContact
    inner join [Framework].[Contact-Active] contact on catalogContact.ContactId = contact.Id
    left join [Framework].[User-Active] userTable on catalogContact.UserId = userTable.Id
where catalogContact.Id = @Id