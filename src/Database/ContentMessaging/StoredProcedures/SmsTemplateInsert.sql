create proc [ContentMessaging].[SmsTemplateInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Title nvarchar(75)
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

declare @sessionOrganizationId uniqueidentifier = (
    select userTable.OrganizationId
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @organizationId uniqueidentifier

select
     @PortalId = coalesce(@PortalId, membership.PortalId)
    ,@organizationId = case
        when membership.Id is not null then membership.OrganizationId
        when portal.OwnerId = @sessionOrganizationId then null
        else @sessionOrganizationId
    end
from [Framework].[Portal-Active] portal
    left join [Content].[Membership-Active] membership on membership.Id = @MembershipId
where portal.Id = coalesce(@PortalId, membership.PortalId)

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if @PortalId is null
    or not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, @PortalId, @organizationId))
    throw 51000, 'SMS template access denied.', 1

begin transaction

insert [Content].[SmsTemplate] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,MembershipId
    ,OrganizationId
    ,Title
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@MembershipId
    ,@organizationId
    ,@Title
    ,@Body
)

commit transaction