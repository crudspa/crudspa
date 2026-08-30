create proc [FrameworkAuth].[AuthFailureInsert] (
     @CorrelationId uniqueidentifier
    ,@Provider nvarchar(75)
    ,@Audience nvarchar(25)
    ,@Reason nvarchar(75)
) as

insert [Framework].[AuthEvent] (
     Id
    ,Created
    ,CorrelationId
    ,Type
    ,Outcome
    ,Provider
    ,Audience
    ,Reason
)
values (
     newid()
    ,sysdatetimeoffset()
    ,@CorrelationId
    ,N'auth-completed'
    ,N'rejected'
    ,@Provider
    ,@Audience
    ,@Reason
)