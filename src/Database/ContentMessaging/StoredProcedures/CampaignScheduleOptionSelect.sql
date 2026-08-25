create proc [ContentMessaging].[CampaignScheduleOptionSelect] (
     @SessionId uniqueidentifier
    ,@DistrictOrganizationId uniqueidentifier
) as

set nocount on

declare @districtPortalId uniqueidentifier = '18da2a92-c650-42fb-8ff9-07c81ab5b9b2'

if not exists (
    select 1 from [ContentMessaging].[SessionCanReadOrganization](@SessionId, @districtPortalId, @DistrictOrganizationId)
)
    throw 51000, 'Campaign schedule access denied.', 1

;with DistrictSchools as (
    select school.Id, school.OrganizationId, organization.Name
    from [Education].[District-Active] district
        inner join [Education].[School-Active] school on school.DistrictId = district.Id
        inner join [Framework].[Organization-Active] organization on organization.Id = school.OrganizationId
    where district.OrganizationId = @DistrictOrganizationId
), AvailableGrades as (
    select distinct classroom.SchoolId, classroom.GradeId
    from [Education].[Classroom-Active] classroom
        inner join DistrictSchools school on school.Id = classroom.SchoolId
    where classroom.GradeId is not null
)
select
     @DistrictOrganizationId as DistrictOrganizationId
    ,scope.OrganizationId
    ,scope.OrganizationName
    ,scope.GradeId
    ,grade.Name as GradeName
from (
    select distinct @DistrictOrganizationId as OrganizationId, N'District-wide' as OrganizationName, GradeId
    from AvailableGrades
    union all
    select school.OrganizationId, school.Name, available.GradeId
    from AvailableGrades available
        inner join DistrictSchools school on school.Id = available.SchoolId
) scope
    inner join [Education].[Grade-Active] grade on grade.Id = scope.GradeId
order by case when scope.OrganizationId = @DistrictOrganizationId then 0 else 1 end,
    grade.Ordinal, scope.OrganizationName