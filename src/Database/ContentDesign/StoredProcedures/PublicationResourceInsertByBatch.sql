create proc [ContentDesign].[PublicationResourceInsertByBatch] (
     @SessionId uniqueidentifier
    ,@PublicationId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@Label nvarchar(150)
    ,@Url nvarchar(2000)
    ,@PdfId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@Ordinal int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
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

insert [Content].[PublicationResource] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PublicationId
    ,TypeId
    ,Label
    ,Url
    ,PdfId
    ,ImageId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PublicationId
    ,@TypeId
    ,@Label
    ,@Url
    ,@PdfId
    ,@ImageId
    ,@Ordinal
)

commit transaction