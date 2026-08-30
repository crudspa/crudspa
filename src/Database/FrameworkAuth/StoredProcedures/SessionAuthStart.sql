create proc [FrameworkAuth].[SessionAuthStart] (
     @SessionId uniqueidentifier
    ,@UserId uniqueidentifier
    ,@Provider nvarchar(75)
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
    ,@portalId uniqueidentifier
    ,@audience nvarchar(25)
    ,@organizationId uniqueidentifier
    ,@authPolicyId uniqueidentifier
    ,@idleTimeoutMinutes int
    ,@absoluteTimeoutMinutes int

begin transaction

select
     @portalId = session.PortalId
    ,@audience = portal.[Key]
    ,@organizationId = [user].OrganizationId
from [Framework].[Session-Active] session with (updlock, holdlock)
    inner join [Framework].[User-Active] [user]
        on session.UserId = [user].Id
        and session.PortalId = [user].PortalId
    inner join [Framework].[Portal-Active] portal on session.PortalId = portal.Id
where session.Id = @SessionId
    and session.UserId = @UserId

if @audience = N'school'
    select @organizationId = district.OrganizationId
    from [Education].[School-Active] school
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
    where school.OrganizationId = @organizationId
else if @audience = N'student'
    select @organizationId = district.OrganizationId
    from [Education].[Student-Active] student
        inner join [Education].[Family-Active] family on student.FamilyId = family.Id
        inner join [Education].[School-Active] school on family.SchoolId = school.Id
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
    where student.UserId = @UserId

declare @policies table (
     Id uniqueidentifier not null primary key
    ,IdleTimeoutMinutes int not null
    ,AbsoluteTimeoutMinutes int not null
)

insert @policies
select policy.Id, policy.IdleTimeoutMinutes, policy.AbsoluteTimeoutMinutes
from [Framework].[AuthPolicy-Active] policy
    inner join [Framework].[AuthConnection-Active] connection
        on policy.AuthConnectionId = connection.Id
        and policy.OrganizationId = connection.OrganizationId
        and connection.Enabled = 1
where policy.OrganizationId = @organizationId
    and policy.Audience = @audience
    and policy.Enabled = 1
    and connection.Provider = @Provider

if (select count(*) from @policies) = 1
    select
         @authPolicyId = Id
        ,@idleTimeoutMinutes = IdleTimeoutMinutes
        ,@absoluteTimeoutMinutes = AbsoluteTimeoutMinutes
    from @policies

if @authPolicyId is not null
    and @idleTimeoutMinutes > 0
    and @absoluteTimeoutMinutes >= @idleTimeoutMinutes
    and not exists (select 1 from [Framework].[SessionAuth] where SessionId = @SessionId)
begin
    insert [Framework].[SessionAuth] (
         Id
        ,SessionId
        ,Provider
        ,Authenticated
        ,LastActivity
        ,IdleTimeoutMinutes
        ,IdleExpires
        ,AbsoluteExpires
        ,AuthPolicyId
    )
    values (
         newid()
        ,@SessionId
        ,@Provider
        ,@now
        ,@now
        ,@idleTimeoutMinutes
        ,dateadd(minute, @idleTimeoutMinutes, @now)
        ,dateadd(minute, @absoluteTimeoutMinutes, @now)
        ,@authPolicyId
    )

    insert [Framework].[AuthEvent] (
         Id, Created, CorrelationId, Type, Outcome, Provider, Audience, PortalId, SessionId
    )
    values (
         newid(), @now, @SessionId, N'session-started', N'succeeded', @Provider, @audience, @portalId, @SessionId
    )
end

commit transaction

select case when exists (
    select 1 from [Framework].[SessionAuth-Active] where SessionId = @SessionId
) then 1 else 0 end