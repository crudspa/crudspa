create proc [ContentMessaging].[CampaignDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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

begin transaction

update campaignLicense
set
     IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[CampaignLicense] campaignLicense
    inner join [Content].[CampaignLicense-Active] activeCampaignLicense on activeCampaignLicense.Id = campaignLicense.Id
where activeCampaignLicense.CampaignId = @Id

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Campaign] baseTable
    inner join [Content].[Campaign-Active] campaign on campaign.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction