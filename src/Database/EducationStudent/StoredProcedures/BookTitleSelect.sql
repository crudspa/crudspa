create proc [EducationStudent].[BookTitleSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
    ,@Title nvarchar(150) output
) as

set @Title = (
    select book.Title
    from [Education].[Book-Active] book
    where book.Id = @Id
        and exists (
            select 1
            from [EducationStudent].[LicensedBooks](@SessionId, book.Id) licensedBook
        )
)