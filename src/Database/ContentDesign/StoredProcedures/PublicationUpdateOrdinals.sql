create proc [ContentDesign].[PublicationUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction
if (select count(*) from @Orderables) != (select count(distinct Id) from @Orderables)
    or exists (select 1 from @Orderables ids where not exists (
        select 1 from Content.[Publication-Active] target
            inner join Framework.[Portal-Active] portal on portal.Id = target.PortalId
            inner join Framework.[User-Active] userTable on userTable.OrganizationId = portal.OwnerId
            inner join Framework.[Session-Active] session on session.UserId = userTable.Id
        where target.Id = ids.Id and session.Id = @SessionId))
    or (select count(distinct target.PortalId) from Content.[Publication-Active] target inner join @Orderables ids on ids.Id = target.Id) > 1
    throw 50000, 'Ordering requires records from one authorized site.', 1;

update publication
set
     publication.Ordinal = orderable.Ordinal
    ,publication.Updated = @now
    ,publication.UpdatedBy = @SessionId
from [Content].[Publication] publication
    inner join @Orderables orderable on orderable.Id = publication.Id
where publication.Ordinal != orderable.Ordinal

commit transaction