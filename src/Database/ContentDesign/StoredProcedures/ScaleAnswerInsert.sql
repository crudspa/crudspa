create proc [ContentDesign].[ScaleAnswerInsert] (
     @SessionId uniqueidentifier
    ,@QuestionId uniqueidentifier
    ,@Kind int
    ,@RatingKind int
    ,@LikertKind int
    ,@RatingMin int
    ,@RatingMax int
    ,@Ordering int
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

insert [Content].[ScaleAnswer] (
     Id, VersionOf, Updated, UpdatedBy, QuestionId, Kind, RatingKind, LikertKind, RatingMin, RatingMax, Ordering
)
values (
     @Id, @Id, @now, @SessionId, @QuestionId, @Kind, @RatingKind, @LikertKind, isnull(@RatingMin, 1), isnull(@RatingMax, 5), @Ordering
)

commit transaction