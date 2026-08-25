create proc [ContentDisplay].[ForumRunUploadStageDiscardExpired] (
    @BlobId uniqueidentifier
) as
begin
    set nocount on;

    delete [Content].[ForumUploadStage]
    where BlobId = @BlobId
        and Consumed is null
        and Expires <= sysdatetimeoffset();
end