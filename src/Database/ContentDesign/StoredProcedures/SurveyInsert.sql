create proc [ContentDesign].[SurveyInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier

    ,@Licenses Framework.IdList readonly
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

insert [Content].[Survey] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
)

if not exists (
    select 1
    from [Content].[Survey-Active] survey
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where survey.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

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
    cross apply (select newid() as JunctionId) newRow

commit transaction