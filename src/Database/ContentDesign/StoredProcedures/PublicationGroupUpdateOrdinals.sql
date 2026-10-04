create proc [ContentDesign].[PublicationGroupUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction
if (select count(*) from @Orderables) != (select count(distinct Id) from @Orderables)
    or exists (select 1 from @Orderables ids where not exists (
        select 1 from Content.[PublicationGroup-Active] target
            inner join Framework.[Portal-Active] portal on portal.Id = target.PortalId
            inner join Framework.[User-Active] userTable on userTable.OrganizationId = portal.OwnerId
            inner join Framework.[Session-Active] session on session.UserId = userTable.Id
        where target.Id = ids.Id and session.Id = @SessionId))
    or (select count(distinct target.PortalId) from Content.[PublicationGroup-Active] target inner join @Orderables ids on ids.Id = target.Id) > 1
    throw 50000, 'Ordering requires records from one authorized site.', 1;

update publicationGroup
set
     publicationGroup.Ordinal = orderable.Ordinal
    ,publicationGroup.Updated = @now
    ,publicationGroup.UpdatedBy = @SessionId
from [Content].[PublicationGroup] publicationGroup
    inner join @Orderables orderable on orderable.Id = publicationGroup.Id
where publicationGroup.Ordinal != orderable.Ordinal

commit transaction