create proc [ContentMessaging].[SmsPreferenceInsert] (
     @SessionId uniqueidentifier
    ,@PortalId uniqueidentifier
    ,@ContactId uniqueidentifier
    ,@ContactPhoneId uniqueidentifier
    ,@Number nvarchar(20)
    ,@Status int
    ,@Source int
    ,@StatusChanged datetimeoffset(7)
    ,@Notes nvarchar(max)
    ,@Id uniqueidentifier output
) as

declare @sessionOrganizationId uniqueidentifier = (
    select userTable.OrganizationId
    from [Framework].[Session-Active] session
        inner join [Framework].[User-Active] userTable on session.UserId = userTable.Id
    where session.Id = @SessionId
)
declare @portalOwnerId uniqueidentifier = (select OwnerId from [Framework].[Portal-Active] where Id = @PortalId)
declare @organizationId uniqueidentifier = case when @sessionOrganizationId = @portalOwnerId then null else @sessionOrganizationId end

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on

if not exists (select 1 from [ContentMessaging].[SessionCanWriteOrganization](@SessionId, @PortalId, @organizationId))
    throw 51000, 'SMS preference access denied.', 1

begin transaction

insert [Content].[SmsPreference] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,PortalId
    ,OrganizationId
    ,ContactId
    ,ContactPhoneId
    ,Number
    ,Status
    ,Source
    ,StatusChanged
    ,Notes
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@PortalId
    ,@organizationId
    ,@ContactId
    ,@ContactPhoneId
    ,@Number
    ,@Status
    ,@Source
    ,@StatusChanged
    ,@Notes
)

commit transaction