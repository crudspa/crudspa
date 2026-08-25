create proc [ContentDisplay].[ForumRunUploadStageSelectExpired]
as
begin
    set nocount on;

    delete top (1000) [Content].[ForumUploadStage]
    where Consumed < dateadd(day, -1, sysdatetimeoffset());

    select top (100) BlobId
    from [Content].[ForumUploadStage] with (readpast)
    where Consumed is null
        and Expires <= sysdatetimeoffset()
    order by Expires;
end