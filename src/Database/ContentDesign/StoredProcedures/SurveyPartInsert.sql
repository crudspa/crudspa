create proc [ContentDesign].[SurveyPartInsert] (
     @SessionId uniqueidentifier
    ,@SurveyId uniqueidentifier
    ,@Title nvarchar(75)
    ,@Instructions nvarchar(max)
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
declare @ordinal int = (select count(1) from [Content].[SurveyPart-Active] where SurveyId = @SurveyId)

set nocount on
set xact_abort on
begin transaction

insert [Content].[SurveyPart] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,SurveyId
    ,Title
    ,Instructions
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@SurveyId
    ,@Title
    ,@Instructions
    ,@ordinal
)

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

commit transaction