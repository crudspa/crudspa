create proc [ContentDesign].[SurveyUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@Description nvarchar(max)
    ,@StatusId uniqueidentifier
    ,@AssignmentKind int
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

if not exists (
    select 1
    from [Content].[Survey-Active] survey
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where survey.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update survey
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,Description = @Description
    ,StatusId = @StatusId
    ,AssignmentKind = @AssignmentKind
from [Content].[Survey] survey
where survey.Id = @Id

commit transaction