create proc [ContentDesign].[NoteImageDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()
declare @noteId uniqueidentifier = (select top 1 NoteId from [Content].[NoteImage] where Id = @Id)

update [Content].[NoteImage]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id

;with cte as (
    select Id, Ordinal, row_number() over (order by Ordinal) as RowNumber
    from [Content].[NoteImage-Active]
    where NoteId = @noteId
)
update cte
set Ordinal = cte.RowNumber - 1