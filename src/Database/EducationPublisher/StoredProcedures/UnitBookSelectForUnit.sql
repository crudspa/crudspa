create proc [EducationPublisher].[UnitBookSelectForUnit] (
     @SessionId uniqueidentifier
    ,@UnitId uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on
select
     unitBook.Id
    ,unitBook.UnitId
    ,unitBook.BookId
    ,unitBook.Treatment
    ,unitBook.Control
    ,unitBook.Ordinal
    ,book.[Key] as BookKey
    ,book.Title as BookTitle
    ,bookCoverImage.Id as BookCoverImageId
    ,bookCoverImage.BlobId as BookCoverImageBlobId
    ,bookCoverImage.Name as BookCoverImageName
    ,bookCoverImage.Format as BookCoverImageFormat
    ,bookCoverImage.Width as BookCoverImageWidth
    ,bookCoverImage.Height as BookCoverImageHeight
    ,bookCoverImage.Caption as BookCoverImageCaption
from [Education].[UnitBook-Active] unitBook
    inner join [Education].[Book-Active] book on unitBook.BookId = book.Id
    inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
    inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    inner join [Framework].[ImageFile-Active] bookCoverImage on book.CoverImageId = bookCoverImage.Id
where unitBook.UnitId = @UnitId
    and organization.Id = @organizationId