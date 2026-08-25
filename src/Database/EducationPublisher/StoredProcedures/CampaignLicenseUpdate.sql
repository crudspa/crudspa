create proc [EducationPublisher].[CampaignLicenseUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@CampaignId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

if exists (
    select 1
    from [Content].[CampaignLicense-Active] with (updlock, holdlock)
    where Id != @Id
        and CampaignId = @CampaignId
        and LicenseId = (select LicenseId from [Content].[CampaignLicense-Active] where Id = @Id)
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,CampaignId = @CampaignId
from [Content].[CampaignLicense] baseTable
    inner join [Content].[CampaignLicense-Active] campaignLicense on campaignLicense.Id = baseTable.Id
    inner join [Content].[Campaign-Active] campaign on campaignLicense.CampaignId = campaign.Id
    inner join [Framework].[License-Active] license on campaignLicense.LicenseId = license.Id
    inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
where baseTable.Id = @Id
    and portal.OwnerId = @organizationId
    and license.OwnerId = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction