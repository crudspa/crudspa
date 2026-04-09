create proc [EducationPublisher].[ListenQuestionUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Text nvarchar(max)
    ,@AudioFileId uniqueidentifier
    ,@IsPreview bit
    ,@PageBreakBefore bit
    ,@HasCorrectChoice bit
    ,@RequireTextInput bit
    ,@CategoryId uniqueidentifier
    ,@TypeId uniqueidentifier
    ,@ImageFileId uniqueidentifier
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

update listenQuestion
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Text = @Text
    ,AudioFileId = @AudioFileId
    ,IsPreview = @IsPreview
    ,PageBreakBefore = @PageBreakBefore
    ,HasCorrectChoice = @HasCorrectChoice
    ,RequireTextInput = @RequireTextInput
    ,CategoryId = @CategoryId
    ,TypeId = @TypeId
    ,ImageFileId = @ImageFileId
from [Education].[ListenQuestion] listenQuestion
where listenQuestion.Id = @Id

commit transaction