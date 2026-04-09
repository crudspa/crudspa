create proc [ContentDesign].[StyleUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@RuleId uniqueidentifier
    ,@ConfigJson nvarchar(max)
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,RuleId = @RuleId
    ,ConfigJson = @ConfigJson
from [Content].[Style] baseTable
    inner join [Content].[Style-Active] style on style.Id = baseTable.Id
    inner join [Content].[ContentPortal-Active] contentPortal on style.ContentPortalId = contentPortal.Id
    inner join [Framework].[Portal-Active] portal on contentPortal.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

declare @contentPortalId uniqueidentifier = (select top 1 ContentPortalId from [Content].[Style] where Id = @Id)

update [Content].[ContentPortal]
set  Updated = @now
    ,UpdatedBy = @SessionId
    ,StyleRevision = StyleRevision + 1
where Id = @contentPortalId

commit transaction