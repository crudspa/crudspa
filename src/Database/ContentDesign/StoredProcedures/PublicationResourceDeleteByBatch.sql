create proc [ContentDesign].[PublicationResourceDeleteByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set xact_abort on
set nocount on
begin transaction
declare @PublicationId uniqueidentifier = (select PublicationId from Content.[PublicationResource-Active] where Id = @Id)
if not exists (select 1 from Content.[Publication-Active] publication
    inner join Framework.[Portal-Active] portal on portal.Id = publication.PortalId
    inner join Framework.[User-Active] userTable on userTable.OrganizationId = portal.OwnerId
    inner join Framework.[Session-Active] session on session.UserId = userTable.Id
    where publication.Id = @PublicationId and session.Id = @SessionId)
    throw 50000, 'Publication resource is outside your scope.', 1;

update [Content].[PublicationResource]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id

commit transaction