create proc [EducationStudent].[StudentAchievementSelectUnlocks] (
     @StudentId uniqueidentifier
    ,@AchievementId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @ConditionGroupControl uniqueidentifier = 'c02332b0-14ee-4897-aa56-4aeba64e8bad'
declare @ConditionGroupTreatment uniqueidentifier = '3befe15f-f053-4f26-826f-583dead12346'

declare @studentConditionGroupId uniqueidentifier
declare @studentGradeId uniqueidentifier
declare @studentAssessmentLevelId uniqueidentifier

select top 1
     @studentConditionGroupId = student.ConditionGroupId
    ,@studentGradeId = student.GradeId
    ,@studentAssessmentLevelId = student.AssessmentLevelGroupId
from [Education].[Student-Active] student
where student.Id = @StudentId

-- Books: Keep WHERE clause in sync with UnitSelectBySession
select
     book.Id
    ,book.Title
    ,bookCoverImage.Id as BookCoverImageId
    ,bookCoverImage.BlobId as BookCoverImageBlobId
    ,bookCoverImage.Name as BookCoverImageName
    ,bookCoverImage.Format as BookCoverImageFormat
    ,bookCoverImage.Width as BookCoverImageWidth
    ,bookCoverImage.Height as BookCoverImageHeight
    ,bookCoverImage.Caption as BookCoverImageCaption
    ,unit.Id as UnitId
from [Education].[Book-Active] book
    inner join [Education].[UnitBook-Active] unitBook on unitBook.BookId = book.Id
    inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
    inner join [Framework].[ImageFile-Active] bookCoverImage on book.CoverImageId = bookCoverImage.Id
where book.StatusId = @ContentStatusComplete
    and ((unitBook.Treatment = 1 and unitBook.Control = 1)
        or (unitBook.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unitBook.Control = 1 and @studentConditionGroupId = @ConditionGroupControl))
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and book.RequiresAchievementId = @AchievementId

-- Games: Keep WHERE clause in sync with parent fetches
select
     game.Id as Id
    ,game.Title as Title
    ,icon.CssClass as IconName
    ,book.Title as BookTitle
    ,bookCoverImage.Id as BookCoverImageId
    ,bookCoverImage.BlobId as BookCoverImageBlobId
    ,bookCoverImage.Name as BookCoverImageName
    ,bookCoverImage.Format as BookCoverImageFormat
    ,bookCoverImage.Width as BookCoverImageWidth
    ,bookCoverImage.Height as BookCoverImageHeight
    ,bookCoverImage.Caption as BookCoverImageCaption
    ,book.Id as BookId
    ,unit.Id as UnitId
from [Education].[Game-Active] game
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    inner join [Framework].[ImageFile-Active] bookCoverImage on book.CoverImageId = bookCoverImage.Id
    inner join [Education].[UnitBook-Active] unitBook on unitBook.BookId = book.Id
    inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
    left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
where game.StatusId = @ContentStatusComplete
    and book.StatusId = @ContentStatusComplete
    and game.GradeId = @studentGradeId
    and game.AssessmentLevelId = @studentAssessmentLevelId
    and ((unitBook.Treatment = 1 and unitBook.Control = 1)
        or (unitBook.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unitBook.Control = 1 and @studentConditionGroupId = @ConditionGroupControl))
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and game.RequiresAchievementId = @AchievementId

-- Lessons: Keep WHERE clause in sync with parent fetches
select
     lesson.Id as Id
    ,lesson.Title as Title
    ,unit.Title as UnitTitle
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,unit.Id as UnitId
from [Education].[Lesson-Active] lesson
    inner join [Framework].[ImageFile-Active] image on lesson.ImageId = image.Id
    inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
where lesson.StatusId = @ContentStatusComplete
    and unit.StatusId = @ContentStatusComplete
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and lesson.RequiresAchievementId = @AchievementId

-- Modules: Keep WHERE clause in sync with parent fetches
select
     module.Id as Id
    ,module.Title as Title
    ,icon.CssClass as IconName
    ,book.Title as BookTitle
    ,bookCoverImage.Id as BookCoverImageId
    ,bookCoverImage.BlobId as BookCoverImageBlobId
    ,bookCoverImage.Name as BookCoverImageName
    ,bookCoverImage.Format as BookCoverImageFormat
    ,bookCoverImage.Width as BookCoverImageWidth
    ,bookCoverImage.Height as BookCoverImageHeight
    ,bookCoverImage.Caption as BookCoverImageCaption
    ,book.Id as BookId
    ,unit.Id as UnitId
from [Education].[Module-Active] module
    inner join [Education].[Book-Active] book on module.BookId = book.Id
    inner join [Education].[UnitBook-Active] unitBook on unitBook.BookId = book.Id
    inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
    inner join [Framework].[ImageFile-Active] bookCoverImage on book.CoverImageId = bookCoverImage.Id
    left join [Framework].[Icon-Active] icon on module.IconId = icon.Id
where module.StatusId = @ContentStatusComplete
    and book.StatusId = @ContentStatusComplete
    and ((unitBook.Treatment = 1 and unitBook.Control = 1)
        or (unitBook.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unitBook.Control = 1 and @studentConditionGroupId = @ConditionGroupControl))
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and module.RequiresAchievementId = @AchievementId

-- Objectives: Keep WHERE clause in sync with parent fetches
select
     objective.Id as Id
    ,objective.Title as Title
    ,lesson.Title as LessonTitle
    ,unit.Title as UnitTitle
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
    ,unitImage.Id as ImageId
    ,unitImage.BlobId as ImageBlobId
    ,unitImage.Name as ImageName
    ,unitImage.Format as ImageFormat
    ,unitImage.Width as ImageWidth
    ,unitImage.Height as ImageHeight
    ,unitImage.Caption as ImageCaption
    ,lesson.Id as LessonId
    ,unit.Id as UnitId
from [Education].[Objective-Active] objective
    inner join [Education].[Lesson-Active] lesson on objective.LessonId = lesson.Id
    inner join [Education].[Unit-Active] unit on lesson.UnitId = unit.Id
    left join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
    left join [Framework].[ImageFile-Active] unitImage on unit.ImageId = unitImage.Id
where objective.StatusId = @ContentStatusComplete
    and lesson.StatusId = @ContentStatusComplete
    and unit.StatusId = @ContentStatusComplete
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and objective.RequiresAchievementId = @AchievementId

-- Trifolds: Keep WHERE clause in sync with fetches
select
     trifold.Id as Id
    ,trifold.Title as Title
    ,book.Title as BookTitle
    ,bookCoverImage.Id as BookCoverImageId
    ,bookCoverImage.BlobId as BookCoverImageBlobId
    ,bookCoverImage.Name as BookCoverImageName
    ,bookCoverImage.Format as BookCoverImageFormat
    ,bookCoverImage.Width as BookCoverImageWidth
    ,bookCoverImage.Height as BookCoverImageHeight
    ,bookCoverImage.Caption as BookCoverImageCaption
    ,book.Id as BookId
    ,unit.Id as UnitId
from [Education].[Trifold-Active] trifold
    inner join [Education].[Book-Active] book on trifold.BookId = book.Id
    inner join [Education].[UnitBook-Active] unitBook on unitBook.BookId = book.Id
    inner join [Education].[Unit-Active] unit on unitBook.UnitId = unit.Id
    inner join [Framework].[ImageFile-Active] bookCoverImage on book.CoverImageId = bookCoverImage.Id
where trifold.StatusId = @ContentStatusComplete
    and book.StatusId = @ContentStatusComplete
    and ((unitBook.Treatment = 1 and unitBook.Control = 1)
        or (unitBook.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unitBook.Control = 1 and @studentConditionGroupId = @ConditionGroupControl))
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and trifold.RequiresAchievementId = @AchievementId

-- Units: Keep WHERE clause in sync with UnitSelectBySession
select
     unit.Id as Id
    ,unit.Title as Title
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
from [Education].[Unit-Active] unit
    inner join [Framework].[ImageFile-Active] image on unit.ImageId = image.Id
where unit.StatusId = @ContentStatusComplete
    and unit.GradeId = @studentGradeId
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and unit.RequiresAchievementId = @AchievementId