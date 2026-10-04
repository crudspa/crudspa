create proc [ContentDesign].[PublicationResourceUpdateByBatch] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@PublicationId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Label nvarchar(150)
    ,@Url nvarchar(2000)
    ,@PdfId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Ordinal int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction
if not exists (select 1 from Content.[Publication-Active] publication
    inner join Framework.[Portal-Active] portal on portal.Id = publication.PortalId
    inner join Framework.[User-Active] userTable on userTable.OrganizationId = portal.OwnerId
    inner join Framework.[Session-Active] session on session.UserId = userTable.Id
    where publication.Id = @PublicationId and session.Id = @SessionId)
    throw 50000, 'Publication resource is outside your scope.', 1;

update [Content].[PublicationResource]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,TypeId = @TypeId
    ,Label = @Label
    ,Url = @Url
    ,PdfId = @PdfId
    ,ImageId = @ImageId
    ,Ordinal = @Ordinal
where Id = @Id and VersionOf = Id and IsDeleted = 0 and PublicationId = @PublicationId
if @@rowcount = 0 throw 50000, 'Publication resource does not belong to this publication.', 1;

commit transaction