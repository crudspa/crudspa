create proc [ContentMessaging].[CampaignInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Name nvarchar(75)
    ,@Description nvarchar(max)
    ,@Licenses [Framework].[IdList] readonly
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (select 1 from [ContentMessaging].[SessionOwnsPortal](@SessionId, @PortalId))
    throw 51000, 'Campaign access denied.', 1

declare @ownerId uniqueidentifier = (
    select OwnerId from [Framework].[Portal-Active] where Id = @PortalId
)

if not exists (select 1 from @Licenses)
    throw 51000, 'At least one Campaign License is required.', 1

if exists (
    select 1
    from @Licenses selectedLicense
        left join [Framework].[License-Active] license on license.Id = selectedLicense.Id
            and license.OwnerId = @ownerId
    where license.Id is null
)
    throw 51000, 'Campaign License access denied.', 1

begin transaction

insert [Content].[Campaign] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,Name
    ,Description
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@Name
    ,@Description
)

insert [Content].[CampaignLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,CampaignId
    ,LicenseId
)
select
     generated.Id
    ,generated.Id
    ,@now
    ,@SessionId
    ,@Id
    ,selectedLicense.Id
from @Licenses selectedLicense
    cross apply (select newid() as Id) generated

commit transaction