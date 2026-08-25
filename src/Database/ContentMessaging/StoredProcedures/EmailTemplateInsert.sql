create proc [ContentMessaging].[EmailTemplateInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@Title nvarchar(75)
    ,@Subject nvarchar(150)
    ,@Body nvarchar(max)
    ,@Id uniqueidentifier output
) as

declare @sessionOrganizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
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
    throw 51000, 'Email template access denied.', 1

begin transaction

insert [Content].[EmailTemplate] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,MembershipId
    ,OrganizationId
    ,Title
    ,Subject
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
    ,@Subject
    ,@Body
)

commit transaction