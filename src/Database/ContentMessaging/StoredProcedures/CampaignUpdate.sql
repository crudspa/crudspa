create proc [ContentMessaging].[CampaignUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@Description nvarchar(max)
    ,@Licenses [Framework].[IdList] readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (
    select 1
    from [Content].[Campaign-Active] campaign
        cross apply [ContentMessaging].[SessionOwnsPortal](@SessionId, campaign.PortalId)
    where campaign.Id = @Id
)
    throw 51000, 'Campaign access denied.', 1

declare @ownerId uniqueidentifier = (
    select portal.OwnerId
    from [Content].[Campaign-Active] campaign
        inner join [Framework].[Portal-Active] portal on portal.Id = campaign.PortalId
    where campaign.Id = @Id
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

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,Description = @Description
from [Content].[Campaign] baseTable
    inner join [Content].[Campaign-Active] campaign on campaign.Id = baseTable.Id
where baseTable.Id = @Id

update campaignLicense
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CampaignLicense] campaignLicense
    inner join [Content].[CampaignLicense-Active] activeCampaignLicense on activeCampaignLicense.Id = campaignLicense.Id
    left join @Licenses selectedLicense on selectedLicense.Id = activeCampaignLicense.LicenseId
where activeCampaignLicense.CampaignId = @Id
    and selectedLicense.Id is null

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
    left join [Content].[CampaignLicense-Active] existing on existing.CampaignId = @Id
        and existing.LicenseId = selectedLicense.Id
    cross apply (select newid() as Id) generated
where existing.Id is null

commit transaction