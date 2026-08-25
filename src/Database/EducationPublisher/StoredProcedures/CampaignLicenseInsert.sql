create proc [EducationPublisher].[CampaignLicenseInsert] (
     @SessionId uniqueidentifier
    ,@LicenseId uniqueidentifier
    ,@CampaignId uniqueidentifier
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

if exists (
    select 1
    from [Content].[CampaignLicense-Active] with (updlock, holdlock)
    where CampaignId = @CampaignId
        and LicenseId = @LicenseId
)
begin
    rollback transaction
    raiserror('An active relationship already exists for this license and content.', 16, 1)
    return
end

insert [Content].[CampaignLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,LicenseId
    ,CampaignId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@LicenseId
    ,@CampaignId
)

if not exists (
    select 1
    from [Content].[CampaignLicense-Active] campaignLicense
        inner join [Content].[Campaign-Active] campaign on campaignLicense.CampaignId = campaign.Id
        inner join [Framework].[License-Active] license on campaignLicense.LicenseId = license.Id
        inner join [Framework].[Portal-Active] portal on campaign.PortalId = portal.Id
    where campaignLicense.Id = @Id
        and portal.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction