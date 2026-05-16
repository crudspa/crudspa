create proc [ContentDesign].[NumberAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@IntegerMin int
    ,@IntegerMax int
    ,@DecimalMin real
    ,@DecimalMax real
    ,@CurrencyMin real
    ,@CurrencyMax real
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[NumberAnswer] (
     Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind, Label, IntegerMin, IntegerMax, DecimalMin, DecimalMax, CurrencyMin, CurrencyMax
)
values (
     @Id, @Id, @now, @SessionId, @QuestionId, @Kind, @Label, @IntegerMin, @IntegerMax, @DecimalMin, @DecimalMax, @CurrencyMin, @CurrencyMax
)

commit transaction