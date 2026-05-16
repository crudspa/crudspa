create proc [ContentDesign].[SurveyPartUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Title nvarchar(75)
    ,@Instructions nvarchar(max)
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
    from [Content].[SurveyPart-Active] part
        inner join [Content].[Survey-Active] survey on part.SurveyId = survey.Id
        inner join [Framework].[Portal-Active] portal on survey.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where part.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

update part
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
    ,Instructions = @Instructions
from [Content].[SurveyPart] part
where part.Id = @Id

commit transaction