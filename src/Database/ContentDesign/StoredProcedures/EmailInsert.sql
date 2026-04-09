create proc [ContentDesign].[EmailInsert] (
     @SessionId uniqueidentifier
    ,@MembershipId uniqueidentifier
    ,@FromName nvarchar(150)
    ,@FromEmail nvarchar(75)
    ,@TemplateId uniqueidentifier
    ,@Send datetimeoffset(7)
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

insert [Content].[Email] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,MembershipId
    ,FromName
    ,FromEmail
    ,TemplateId
    ,Send
    ,Subject
    ,Body
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@MembershipId
    ,@FromName
    ,@FromEmail
    ,@TemplateId
    ,@Send
    ,@Subject
    ,@Body
)

if not exists (
    select 1
    from [Content].[Email-Active] email
        inner join [Content].[Membership-Active] membership on email.MembershipId = membership.Id
        inner join [Framework].[Portal-Active] portal on membership.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where email.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction