create proc [ContentDesign].[CommentDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

update baseTable
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
from [Content].[Comment] baseTable
    inner join [Content].[Comment-Active] comment on comment.Id = baseTable.Id
where baseTable.Id = @Id

commit transaction