create proc [EducationPublisher].[CampaignLicenseDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
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

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
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