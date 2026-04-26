create proc [ContentDesign].[CommentUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Body nvarchar(max)
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set
     Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Body = @Body
from [Content].[Comment] baseTable
    inner join [Content].[Comment-Active] comment on comment.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction