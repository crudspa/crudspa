create proc [FrameworkAuth].[NativeAuthPolicySelect] (
    @UserId uniqueidentifier
) as

set nocount on

declare @audience nvarchar(25)
    ,@organizationId uniqueidentifier
    ,@provider nvarchar(75)
    ,@providerCount int

select
     @audience = portal.[Key]
    ,@organizationId = [user].OrganizationId
from [Framework].[User-Active] [user]
    inner join [Framework].[Portal-Active] portal on portal.Id = [user].PortalId
where [user].Id = @UserId

if @audience = N'provider'
begin
    select N'password-email-code' Provider
    return
end

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

select
     @provider = min(connection.Provider)
    ,@providerCount = count(*)
from [Framework].[AuthPolicy-Active] policy
    inner join [Framework].[AuthConnection-Active] connection
        on connection.Id = policy.AuthConnectionId
        and connection.OrganizationId = policy.OrganizationId
        and connection.Enabled = 1
where policy.OrganizationId = @organizationId
    and policy.Audience = @audience
    and policy.Enabled = 1

if @providerCount = 1
    select @provider Provider
else if @providerCount = 0 and @organizationId is not null
    select case @audience
        when N'student' then N'student-code'
        else N'password-email-code'
    end Provider