create proc [ContentDesign].[DateAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@Label nvarchar(150)
    ,@DateMin date
    ,@DateMax date
    ,@TimeMin time
    ,@TimeMax time
    ,@DateTimeMin datetimeoffset
    ,@DateTimeMax datetimeoffset
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[DateAnswer] (
     Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind, Label, DateMin, DateMax, TimeMin, TimeMax, DateTimeMin, DateTimeMax
)
values (
     @Id, @Id, @now, @SessionId, @QuestionId, @Kind, @Label, @DateMin, @DateMax, @TimeMin, @TimeMax, @DateTimeMin, @DateTimeMax
)

commit transaction