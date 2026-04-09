create proc [EducationPublisher].[VocabQuestionInsert] (
     @SessionId uniqueidentifier
    ,@VocabPartId uniqueidentifier
    ,@Word nvarchar(50)
    ,@AudioFileId uniqueidentifier
    ,@IsPreview bit
    ,@PageBreakBefore bit
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

declare @ordinal int = (select count(1) from [Education].[VocabQuestion-Active] where VocabPartId = @VocabPartId)

insert [Education].[VocabQuestion] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,VocabPartId
    ,Word
    ,AudioFileId
    ,IsPreview
    ,PageBreakBefore
    ,Ordinal
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@VocabPartId
    ,@Word
    ,@AudioFileId
    ,@IsPreview
    ,@PageBreakBefore
    ,@ordinal
)

if not exists (
    select 1
    from [Education].[VocabQuestion-Active] vocabQuestion
        inner join [Education].[VocabPart-Active] vocabPart on vocabQuestion.VocabPartId = vocabPart.Id
        inner join [Education].[Assessment-Active] assessment on vocabPart.AssessmentId = assessment.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    where vocabQuestion.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

commit transaction