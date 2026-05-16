create proc [ContentDisplay].[QuestionReplyInsert] (
     @SessionId uniqueidentifier
    ,@ElementId uniqueidentifier
    ,@SurveyReplyId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Submitted datetimeoffset
    ,@BoolValue bit
    ,@TextValue nvarchar(max)
    ,@HtmlValue nvarchar(max)
    ,@DateValue date
    ,@TimeValue time
    ,@DateTimeValue datetimeoffset
    ,@IntegerValue int
    ,@DecimalValue real
    ,@CurrencyValue real
    ,@OtherBoolValue bit
    ,@OtherTextValue nvarchar(150)
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@PostalId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = null
declare @now datetimeoffset = sysdatetimeoffset()

declare @contactId uniqueidentifier = (
    select top 1 userTable.ContactId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @binderId uniqueidentifier

if (@ElementId is not null)
begin
    select top 1 @binderId = page.BinderId
    from [Content].[Element-Active] element
        inner join [Content].[Section-Active] section on element.SectionId = section.Id
        inner join [Content].[Page-Active] page on section.PageId = page.Id
    where element.Id = @ElementId
end

set nocount on
set xact_abort on
begin transaction

if (@contactId is not null)
begin
    if (@SurveyReplyId is not null
        and not exists (
            select 1
            from [Content].[SurveyReply-Active] surveyReply
            where surveyReply.Id = @SurveyReplyId
                and surveyReply.ContactId = @contactId
                and surveyReply.Terminated is null
                and (
                    (@binderId is not null
                        and surveyReply.BinderId = @binderId
                        and surveyReply.SurveyId is null)
                    or (@binderId is null
                        and surveyReply.SurveyId is not null
                        and surveyReply.BinderId is null
                        and surveyReply.Completed is null
                        and exists (
                            select 1
                            from [Content].[SurveyQuestion-Active] surveyQuestion
                                inner join [Content].[SurveyPart-Active] surveyPart on surveyQuestion.PartId = surveyPart.Id
                            where surveyPart.SurveyId = surveyReply.SurveyId
                                and surveyQuestion.QuestionId = @QuestionId
                        ))
                )
        ))
    begin
        set @SurveyReplyId = null
    end

    if (@SurveyReplyId is null and @binderId is not null)
    begin
        select top 1 @SurveyReplyId = surveyReply.Id
        from [Content].[SurveyReply] surveyReply with (updlock, holdlock)
        where surveyReply.ContactId = @contactId
            and surveyReply.BinderId = @binderId
            and surveyReply.SurveyId is null
            and surveyReply.Terminated is null
            and surveyReply.IsDeleted = 0
            and surveyReply.VersionOf = surveyReply.Id
        order by surveyReply.Started desc
    end

    if (@SurveyReplyId is not null
        and @binderId is null
        and exists (
            select 1
            from [Content].[SurveyReply-Active] surveyReply
            where surveyReply.Id = @SurveyReplyId
                and surveyReply.ContactId = @contactId
                and surveyReply.BinderId is null
                and surveyReply.Completed is not null
        ))
    begin
        set @SurveyReplyId = null
    end

    if (@SurveyReplyId is null and @binderId is not null)
    begin
        set @SurveyReplyId = newid()

        insert [Content].[SurveyReply] (
             Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,SurveyId
            ,BinderId
            ,ContactId
            ,Started
        )
        values (
             @SurveyReplyId
            ,@SurveyReplyId
            ,@now
            ,@SessionId
            ,null
            ,@binderId
            ,@contactId
            ,@now
        )
    end

    if (@SurveyReplyId is not null)
    begin
        set @Id = newid()

        update answerChoiceReply
        set IsDeleted = 1
            ,Updated = @now
            ,UpdatedBy = @SessionId
        from [Content].[AnswerChoiceReply] answerChoiceReply
            inner join [Content].[QuestionReply-Active] questionReply on answerChoiceReply.QuestionReplyId = questionReply.Id
        where questionReply.SurveyReplyId = @SurveyReplyId
            and questionReply.QuestionId = @QuestionId
            and answerChoiceReply.IsDeleted = 0
            and answerChoiceReply.VersionOf = answerChoiceReply.Id

        update [Content].[QuestionReply]
        set IsDeleted = 1
            ,Updated = @now
            ,UpdatedBy = @SessionId
        where SurveyReplyId = @SurveyReplyId
            and QuestionId = @QuestionId
            and IsDeleted = 0
            and VersionOf = Id

        insert [Content].[QuestionReply] (
             Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,SurveyReplyId
            ,QuestionId
            ,Submitted
            ,BoolValue
            ,TextValue
            ,HtmlValue
            ,DateValue
            ,TimeValue
            ,DateTimeValue
            ,IntegerValue
            ,DecimalValue
            ,CurrencyValue
            ,OtherBoolValue
            ,OtherTextValue
            ,AudioId
            ,ImageId
            ,PdfId
            ,VideoId
            ,PostalId
        )
        values (
             @Id
            ,@Id
            ,@now
            ,@SessionId
            ,@SurveyReplyId
            ,@QuestionId
            ,isnull(@Submitted, @now)
            ,@BoolValue
            ,@TextValue
            ,@HtmlValue
            ,@DateValue
            ,@TimeValue
            ,@DateTimeValue
            ,@IntegerValue
            ,@DecimalValue
            ,@CurrencyValue
            ,isnull(@OtherBoolValue, 0)
            ,@OtherTextValue
            ,@AudioId
            ,@ImageId
            ,@PdfId
            ,@VideoId
            ,@PostalId
        )

        if (@binderId is not null
            and not exists (
                select 1
                from [Content].[Element-Active] element
                    inner join [Content].[QuestionElement-Active] questionElement on element.Id = questionElement.ElementId
                    inner join [Content].[Section-Active] section on element.SectionId = section.Id
                    inner join [Content].[Page-Active] page on section.PageId = page.Id
                where page.BinderId = @binderId
                    and element.RequireInteraction = 1
                    and not exists (
                        select 1
                        from [Content].[QuestionReply-Active] reply
                        where reply.SurveyReplyId = @SurveyReplyId
                            and reply.QuestionId = questionElement.QuestionId
                    )
            ))
        begin
            update [Content].[SurveyReply]
            set Completed = isnull(Completed, @now)
                ,Updated = @now
                ,UpdatedBy = @SessionId
            where Id = @SurveyReplyId
                and VersionOf = Id
                and IsDeleted = 0
        end
    end
end

commit transaction