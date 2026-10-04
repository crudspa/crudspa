create proc [ContentDesign].[PublicationInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@GroupId uniqueidentifier
    ,@StatusId uniqueidentifier
    ,@Title nvarchar(300)
    ,@Citation nvarchar(max)
    ,@Venue nvarchar(250)
    ,@Sample nvarchar(500)
    ,@Timeline nvarchar(100)
    ,@Outcome nvarchar(1000)
    ,@Intervention nvarchar(1000)
    ,@ImageId uniqueidentifier
    ,@ImageAlt nvarchar(300)
    ,@Year int
    ,@Id uniqueidentifier output
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if @GroupId is not null and not exists (select 1 from Content.[PublicationGroup-Active] where Id = @GroupId and PortalId = @PortalId)
    throw 50000, 'Publication group must belong to the same site.', 1;
if @StatusId = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c' and nullif(ltrim(rtrim(@Citation)), N'') is null
    throw 50000, 'Citation is required before publication.', 1;
if @Year is not null and (@Year < 1000 or @Year > 9999)
    throw 50000, 'Publication year must have four digits.', 1;


declare @ordinal int = (select count(1) from [Content].[Publication-Active] where PortalId = @PortalId)

insert [Content].[Publication] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,GroupId
    ,StatusId
    ,Title
    ,Citation
    ,Venue
    ,Sample
    ,Timeline
    ,Outcome
    ,Intervention
    ,ImageId
    ,ImageAlt
    ,Year
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@GroupId
    ,@StatusId
    ,@Title
    ,@Citation
    ,@Venue
    ,@Sample
    ,@Timeline
    ,@Outcome
    ,@Intervention
    ,@ImageId
    ,@ImageAlt
    ,@Year
    ,@ordinal
)

if not exists (
    select 1
    from [Content].[Publication-Active] publication
        inner join [Framework].[Portal-Active] portal on publication.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where publication.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction