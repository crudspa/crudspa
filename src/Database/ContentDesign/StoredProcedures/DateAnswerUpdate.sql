create proc [ContentDesign].[DateAnswerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@DateMin date
    ,@DateMax date
    ,@TimeMin time
    ,@TimeMax time
    ,@DateTimeMin datetimeoffset
    ,@DateTimeMax datetimeoffset
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[DateAnswer]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
    ,Kind = @Kind
    ,Label = @Label
    ,DateMin = @DateMin
    ,DateMax = @DateMax
    ,TimeMin = @TimeMin
    ,TimeMax = @TimeMax
    ,DateTimeMin = @DateTimeMin
    ,DateTimeMax = @DateTimeMax
where Id = @Id

commit transaction