create proc [EducationPublisher].[ReadQuestionInsert] (
     @SessionId uniqueidentifier
    ,@ReadPartId uniqueidentifier
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

declare @ordinal int = (select count(1) from [Education].[ReadQuestion-Active] where ReadPartId = @ReadPartId)

insert [Education].[ReadQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ReadPartId
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
    ,@ReadPartId
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
    from [Education].[ReadQuestion-Active] readQuestion
        inner join [Education].[ReadPart-Active] readPart on readQuestion.ReadPartId = readPart.Id
        inner join [Education].[Assessment-Active] assessment on readPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where readQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction