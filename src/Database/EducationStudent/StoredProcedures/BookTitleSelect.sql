create proc [EducationStudent].[BookTitleSelect] (
     @Id uniqueidentifier
    ,@Title nvarchar(150) output
) as

set @Title = (select book.Title from [Education].[Book-Active] book where book.Id = @Id)