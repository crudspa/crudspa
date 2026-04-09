create proc [EducationStudent].[UnitSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as
set nocount on

declare @now datetimeoffset = sysdatetimeoffset()

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @ConditionGroupControl uniqueidentifier = 'c02332b0-14ee-4897-aa56-4aeba64e8bad'
declare @ConditionGroupTreatment uniqueidentifier = '3befe15f-f053-4f26-826f-583dead12346'

declare @studentId uniqueidentifier
declare @studentConditionGroupId uniqueidentifier
declare @studentGradeId uniqueidentifier
declare @studentAssessmentLevelId uniqueidentifier

select
     @studentId = student.Id
    ,@studentConditionGroupId = student.ConditionGroupId
    ,@studentGradeId = student.GradeId
    ,@studentAssessmentLevelId = student.AssessmentLevelGroupId
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] users on users.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = users.Id
        and session.Id = @SessionId
option (recompile)

insert [Education].[UnitViewed] (
     Id
    ,Updated
    ,UpdatedBy
    ,UnitId
)
values (
     newid()
    ,@now
    ,@SessionId
    ,@Id
)

create table #StudentAchievements (
    AchievementId uniqueidentifier not null primary key clustered
)

insert #StudentAchievements (AchievementId)
select distinct sa.AchievementId
from [Education].[StudentAchievement-Active] sa
where sa.StudentId = @studentId

create table #ApplicableUnitLicense (
     Id uniqueidentifier not null primary key clustered
    ,AllLessons bit not null
    ,AllBooks bit not null
)

insert #ApplicableUnitLicense (Id, AllLessons, AllBooks)
select distinct
     unitLicense.Id
    ,unitLicense.AllLessons
    ,unitLicense.AllBooks
from [EducationStudent].[UnitLicenses](@SessionId, @Id) unitLicense

create table #LicensedUnits (
    Id uniqueidentifier not null primary key clustered
)

create table #LicensedLessons (
    Id uniqueidentifier not null primary key clustered
)

create table #LicensedBooks (
    Id uniqueidentifier not null primary key clustered
)

insert #LicensedUnits (Id)
select childUnit.Id
from [Education].[Unit-Active] childUnit
where childUnit.ParentId = @Id
    and exists (
        select 1
        from [EducationStudent].[UnitLicenses](@SessionId, childUnit.Id) unitLicense
    )

if exists (select 1 from #ApplicableUnitLicense where AllLessons = 1)
begin
    insert #LicensedLessons (Id)
    select lesson.Id
    from [Education].[Lesson-Active] lesson
    where lesson.UnitId = @Id
end
else
begin
    insert #LicensedLessons (Id)
    select distinct lesson.Id
    from [Education].[Lesson-Active] lesson
        inner join [Education].[UnitLicenseLesson-Active] unitLicenseLesson
            on unitLicenseLesson.LessonId = lesson.Id
        inner join #ApplicableUnitLicense unitLicense
            on unitLicense.Id = unitLicenseLesson.UnitLicenseId
    where lesson.UnitId = @Id
end

if exists (select 1 from #ApplicableUnitLicense where AllBooks = 1)
begin
    insert #LicensedBooks (Id)
    select distinct unitBook.BookId
    from [Education].[UnitBook-Active] unitBook
    where unitBook.UnitId = @Id
end
else
begin
    insert #LicensedBooks (Id)
    select distinct unitBook.BookId
    from [Education].[UnitBook-Active] unitBook
        inner join [Education].[UnitLicenseBook-Active] unitLicenseBook
            on unitLicenseBook.BookId = unitBook.BookId
        inner join #ApplicableUnitLicense unitLicense
            on unitLicense.Id = unitLicenseBook.UnitLicenseId
    where unitBook.UnitId = @Id
end

create table #LessonObjectiveCount (
     LessonId uniqueidentifier not null primary key clustered
    ,ObjectiveCount int not null
)

insert #LessonObjectiveCount (LessonId, ObjectiveCount)
select
     objective.LessonId
    ,count_big(1)
from [Education].[Objective-Active] objective
    inner join #LicensedLessons licensedLesson on licensedLesson.Id = objective.LessonId
group by objective.LessonId

create table #BookFlags (
     BookId uniqueidentifier not null primary key clustered
    ,HasPreface bit not null
    ,HasContent bit not null
    ,HasMap bit not null
)

insert #BookFlags (BookId, HasPreface, HasContent, HasMap)
select
     licensedBook.Id as BookId
    ,convert(bit, case when exists (
        select 1
        from [Education].[Book-Active] bookPreface
            inner join [Content].[Page-Active] page on page.BinderId = bookPreface.PrefaceBinderId
        where bookPreface.Id = licensedBook.Id
            and page.StatusId = @ContentStatusComplete
    ) then 1 else 0 end) as HasPreface
    ,convert(bit, case when exists (
        select 1
        from [Education].[Chapter-Active] chapter
            inner join [Content].[Page-Active] page on page.BinderId = chapter.BinderId
        where chapter.BookId = licensedBook.Id
            and page.StatusId = @ContentStatusComplete
    ) then 1 else 0 end) as HasContent
    ,convert(bit, case when exists (
        select 1
        from [Education].[Trifold-Active] trifold
            inner join [Content].[Page-Active] page on page.BinderId = trifold.BinderId
        where trifold.BookId = licensedBook.Id
            and trifold.StatusId = @ContentStatusComplete
            and page.StatusId = @ContentStatusComplete
    ) then 1 else 0 end) as HasMap
from #LicensedBooks licensedBook

select
     unit.Id
    ,unit.Title
    ,unit.StatusId
    ,unit.GradeId
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
from [Education].[Unit-Active] unit
    inner join [Framework].[ImageFile-Active] image on unit.ImageId = image.Id
where unit.Id = @Id

select
     lesson.Id
    ,lesson.Title
    ,lesson.ImageId
    ,lesson.GuideImageId
    ,lesson.GuideText
    ,lesson.GuideAudioId
    ,lesson.RequireSequentialCompletion
    ,lesson.Ordinal
    ,status.Name as StatusName
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,isnull(objectiveCount.ObjectiveCount, 0) as ObjectiveCount
from [Education].[Lesson-Active] lesson
    inner join [Framework].[ContentStatus-Active] status on lesson.StatusId = status.Id
    inner join [Framework].[ImageFile-Active] image on lesson.ImageId = image.Id
    inner join #LicensedLessons licensedLesson on licensedLesson.Id = lesson.Id
    left join #LessonObjectiveCount objectiveCount on objectiveCount.LessonId = lesson.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = lesson.RequiresAchievementId
where lesson.UnitId = @Id
    and lesson.StatusId = @ContentStatusComplete
    and ((lesson.Treatment = 1 and lesson.Control = 1)
        or (lesson.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (lesson.Control = 1 and @studentConditionGroupId = @ConditionGroupControl))
    and (lesson.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)
order by lesson.Ordinal

select
     unitBook.Id
    ,unitBook.UnitId
    ,unitBook.BookId
    ,unitBook.Treatment
    ,unitBook.Control
    ,unitBook.Ordinal
    ,book.Title as BookTitle
    ,book.Author as BookAuthor
    ,book.CoverImageId as BookCoverImageId
    ,bookCoverImage.Id as BookCoverImageId
    ,bookCoverImage.BlobId as BookCoverImageBlobId
    ,bookCoverImage.Name as BookCoverImageName
    ,bookCoverImage.Format as BookCoverImageFormat
    ,bookCoverImage.Width as BookCoverImageWidth
    ,bookCoverImage.Height as BookCoverImageHeight
    ,bookCoverImage.Caption as BookCoverImageCaption
    ,flags.HasPreface
    ,flags.HasContent
    ,flags.HasMap
from [Education].[UnitBook-Active] unitBook
    inner join [Education].[Book-Active] book on unitBook.BookId = book.Id
    inner join [Framework].[ImageFile-Active] bookCoverImage on book.CoverImageId = bookCoverImage.Id
    inner join #LicensedBooks licensedBook on licensedBook.Id = book.Id
    left join #BookFlags flags on flags.BookId = book.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = book.RequiresAchievementId
where unitBook.UnitId = @Id
    and book.StatusId = @ContentStatusComplete
    and ((unitBook.Treatment = 1 and unitBook.Control = 1)
        or (unitBook.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unitBook.Control = 1 and @studentConditionGroupId = @ConditionGroupControl))
    and (book.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)

select
     unit.Id
    ,unit.Title
    ,unit.GeneratesAchievementId
    ,unit.RequiresAchievementId
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
from [Education].[Unit-Active] unit
    inner join [Framework].[ImageFile-Active] image on unit.ImageId = image.Id
    inner join #LicensedUnits licensedUnit on licensedUnit.Id = unit.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = unit.RequiresAchievementId
where unit.StatusId = @ContentStatusComplete
    and unit.GradeId = @studentGradeId
    and unit.ParentId = @Id
    and ((unit.Treatment = 1 and unit.Control = 1)
        or (unit.Treatment = 1 and @studentConditionGroupId = @ConditionGroupTreatment)
        or (unit.Control = 1 and @studentConditionGroupId = @ConditionGroupControl)
    )
    and (unit.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)
order by unit.Ordinal, unit.Title

select
     objective.Id
    ,objective.Title
    ,objective.LessonId
    ,objective.BinderId
    ,objective.Ordinal
    ,trophyImage.Id as TrophyImageId
    ,trophyImage.BlobId as TrophyImageBlobId
    ,trophyImage.Name as TrophyImageName
    ,trophyImage.Format as TrophyImageFormat
    ,trophyImage.Width as TrophyImageWidth
    ,trophyImage.Height as TrophyImageHeight
    ,trophyImage.Caption as TrophyImageCaption
from [Education].[Objective-Active] objective
    left join [Framework].[ImageFile-Active] trophyImage on objective.TrophyImageId = trophyImage.Id
    inner join [Education].[Lesson-Active] lesson on objective.LessonId = lesson.Id
    inner join #LicensedLessons licensedLesson on licensedLesson.Id = lesson.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = objective.RequiresAchievementId
where lesson.UnitId = @Id
    and objective.StatusId = @ContentStatusComplete
    and (objective.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)
order by objective.Ordinal

select
     game.Id
    ,game.BookId
    ,game.[Key]
    ,game.Title
    ,icon.CssClass as IconName
from [Education].[Game-Active] game
    left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
    inner join [Education].[Book-Active] book on game.BookId = book.Id
    inner join [Education].[UnitBook-Active] unitBook on unitBook.BookId = book.Id
    inner join #LicensedBooks licensedBook on licensedBook.Id = book.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = game.RequiresAchievementId
where unitBook.UnitId = @Id
    and game.StatusId = @ContentStatusComplete
    and game.GradeId = @studentGradeId
    and game.AssessmentLevelId = @studentAssessmentLevelId
    and (game.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)
order by game.[Key]

select
     module.Id
    ,module.Title
    ,icon.CssClass as IconName
    ,module.BookId
    ,module.StatusId
    ,module.BinderId
    ,module.Ordinal
from [Education].[Module-Active] module
    left join [Framework].[Icon-Active] icon on module.IconId = icon.Id
    inner join [Education].[Book-Active] book on module.BookId = book.Id
    inner join [Education].[UnitBook-Active] unitBook on unitBook.BookId = book.Id
    inner join #LicensedBooks licensedBook on licensedBook.Id = book.Id
    left join #StudentAchievements requiredAchievement on requiredAchievement.AchievementId = module.RequiresAchievementId
where unitBook.UnitId = @Id
    and module.StatusId = @ContentStatusComplete
    and (module.RequiresAchievementId is null or requiredAchievement.AchievementId is not null)