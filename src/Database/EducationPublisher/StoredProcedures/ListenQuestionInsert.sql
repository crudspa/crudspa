create proc [EducationPublisher].[ListenQuestionInsert] (
     @SessionId uniqueidentifier
    ,@ListenPartId uniqueidentifier
    ,@Text nvarchar(max)
    ,@AudioFileId uniqueidentifier
    ,@IsPreview bit
    ,@PageBreakBefore bit
    ,@HasCorrectChoice bit
    ,@RequireTextInput bit
    ,@CategoryId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@ImageFileId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Education].[ListenQuestion-Active] where ListenPartId = @ListenPartId)

insert [Education].[ListenQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ListenPartId
    ,Text
    ,AudioFileId
    ,IsPreview
    ,PageBreakBefore
    ,HasCorrectChoice
    ,RequireTextInput
    ,CategoryId
    ,TypeId
    ,ImageFileId
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ListenPartId
    ,@Text
    ,@AudioFileId
    ,@IsPreview
    ,@PageBreakBefore
    ,@HasCorrectChoice
    ,@RequireTextInput
    ,@CategoryId
    ,@TypeId
    ,@ImageFileId
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[ListenQuestion-Active] listenQuestion
        inner join [Education].[ListenPart-Active] listenPart on listenQuestion.ListenPartId = listenPart.Id
        inner join [Education].[Assessment-Active] assessment on listenPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where listenQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction