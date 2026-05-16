create proc [ContentDesign].[NumberAnswerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@IntegerMin int
    ,@IntegerMax int
    ,@DecimalMin real
    ,@DecimalMax real
    ,@CurrencyMin real
    ,@CurrencyMax real
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[NumberAnswer]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
    ,Kind = @Kind
    ,Label = @Label
    ,IntegerMin = @IntegerMin
    ,IntegerMax = @IntegerMax
    ,DecimalMin = @DecimalMin
    ,DecimalMax = @DecimalMax
    ,CurrencyMin = @CurrencyMin
    ,CurrencyMax = @CurrencyMax
where Id = @Id

commit transaction