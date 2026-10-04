create proc [ContentDesign].[PublicationResourceUpdateOrdinalsByBatch] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction
if (select count(*) from @Orderables) != (select count(distinct Id) from @Orderables)
    or exists (select 1 from @Orderables ids where not exists (
    select 1 from Content.[PublicationResource-Active] resource
        inner join Content.[Publication-Active] publication on publication.Id = resource.PublicationId
        inner join Framework.[Portal-Active] portal on portal.Id = publication.PortalId
        inner join Framework.[User-Active] userTable on userTable.OrganizationId = portal.OwnerId
        inner join Framework.[Session-Active] session on session.UserId = userTable.Id
    where resource.Id = ids.Id and session.Id = @SessionId))
    or (select count(distinct PublicationId) from Content.[PublicationResource-Active] resource inner join @Orderables ids on ids.Id = resource.Id) > 1
    throw 50000, 'Resource ordering requires one authorized publication.', 1;

update publicationResource
set
    publicationResource.Ordinal = orderable.Ordinal
    ,publicationResource.Updated = @now
    ,publicationResource.UpdatedBy = @SessionId
from [Content].[PublicationResource] publicationResource
    inner join @Orderables orderable on orderable.Id = publicationResource.Id
where publicationResource.Ordinal != orderable.Ordinal

commit transaction