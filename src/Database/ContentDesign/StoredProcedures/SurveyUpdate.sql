create proc [ContentDesign].[SurveyUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Licenses Framework.IdList readonly
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
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Survey] baseTable
    inner join [Content].[Survey-Active] survey on survey.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end


update surveyLicense
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[SurveyLicense] surveyLicense
    left join @Licenses ids on ids.Id = surveyLicense.LicenseId
where surveyLicense.SurveyId = @Id
    and surveyLicense.IsDeleted = 0
    and ids.Id is null

insert [Content].[SurveyLicense] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SurveyId
    ,LicenseId
)
select
     newRow.JunctionId
    ,newRow.JunctionId
    ,@now
    ,@SessionId
    ,@Id
    ,ids.Id
from (select distinct Id from @Licenses) ids
    inner join [Framework].[License-Active] license on license.Id = ids.Id
        and license.OwnerId = @organizationId
    left join [Content].[SurveyLicense-Active] existingJunction on existingJunction.SurveyId = @Id
        and existingJunction.LicenseId = ids.Id
    cross apply (select newid() as JunctionId) newRow
where existingJunction.Id is null
commit transaction