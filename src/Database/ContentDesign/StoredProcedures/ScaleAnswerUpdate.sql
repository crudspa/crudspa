create proc [ContentDesign].[ScaleAnswerUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@RatingKind int
    ,@LikertKind int
    ,@RatingMin int
    ,@RatingMax int
    ,@Ordering int
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update [Content].[ScaleAnswer]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,QuestionId = @QuestionId
    ,Kind = @Kind
    ,RatingKind = @RatingKind
    ,LikertKind = @LikertKind
    ,RatingMin = isnull(@RatingMin, 1)
    ,RatingMax = isnull(@RatingMax, 5)
    ,Ordering = @Ordering
where Id = @Id

commit transaction