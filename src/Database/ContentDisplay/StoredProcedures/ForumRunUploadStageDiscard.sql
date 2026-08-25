create proc [ContentDisplay].[ForumRunUploadStageDiscard] (
     @SessionId uniqueidentifier
    ,@BlobId uniqueidentifier
) as
begin
    set nocount on;
    set xact_abort on;

    declare @sessionUserId uniqueidentifier;

    select @sessionUserId = session.UserId
    from [Framework].[Session-Active] session
    inner join [Framework].[User-Active] userTable on userTable.Id = session.UserId
    where session.Id = @SessionId and session.Ended is null;

    delete uploadStage
    from [Content].[ForumUploadStage] uploadStage
    where uploadStage.BlobId = @BlobId
        and uploadStage.SessionId = @SessionId
        and uploadStage.UserId = @sessionUserId
        and uploadStage.Consumed is null;
end