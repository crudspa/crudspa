create proc [ContentDesign].[EmailTemplateInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@Title nvarchar(75)
    ,@Subject nvarchar(150)
    ,@Body nvarchar(max)
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

insert [Content].[EmailTemplate] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MembershipId
    ,Title
    ,Subject
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MembershipId
    ,@Title
    ,@Subject
    ,@Body
)

if not exists (
    select 1
    from [Content].[EmailTemplate-Active] emailTemplate
        inner join [Content].[Membership-Active] membership on emailTemplate.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where emailTemplate.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction