create proc [FrameworkAuth].[ExternalAuthPolicySelect] (
    @UserId uniqueidentifier
) as

set nocount on

declare @audience nvarchar(25)
    ,@organizationId uniqueidentifier

select
     @audience = portal.[Key]
    ,@organizationId = [user].OrganizationId
from [Framework].[User-Active] [user]
inner join [Framework].[Portal-Active] portal on portal.Id = [user].PortalId
where [user].Id = @UserId

if @audience = N'school'
    select @organizationId = district.OrganizationId
    from [Education].[School-Active] school
    inner join [Education].[District-Active] district on district.Id = school.DistrictId
    where school.OrganizationId = @organizationId
else if @audience = N'student'
    select @organizationId = district.OrganizationId
    from [Education].[Student-Active] student
    inner join [Education].[Family-Active] family on family.Id = student.FamilyId
    inner join [Education].[School-Active] school on school.Id = family.SchoolId
    inner join [Education].[District-Active] district on district.Id = school.DistrictId
    where student.UserId = @UserId

declare @routes table (
     Provider nvarchar(75) not null
    ,Tenant nvarchar(255) not null
    ,Audience nvarchar(25) not null
)

insert @routes
select connection.Provider, connection.Tenant, policy.Audience
from [Framework].[AuthPolicy-Active] policy
inner join [Framework].[AuthConnection-Active] connection on connection.Id = policy.AuthConnectionId
    and connection.OrganizationId = policy.OrganizationId
    and connection.Enabled = 1
where policy.OrganizationId = @organizationId
    and policy.Audience = @audience
    and policy.Enabled = 1
    and connection.Tenant is not null
    and connection.Provider not in (N'password-email-code', N'email-code', N'student-code')

if (select count(*) from @routes) = 1
    select Provider, Tenant, Audience from @routes