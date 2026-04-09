create proc [EducationStudent].[BookLiteSelect] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'

insert [Education].[BookViewed] (
     Id
    ,BookId
    ,UpdatedBy
)
values (
     newid()
    ,@Id
    ,@SessionId
)

declare @studentId uniqueidentifier
declare @studentGradeId uniqueidentifier
declare @studentAssessmentLevelId uniqueidentifier

select top 1
     @studentId = student.Id
    ,@studentGradeId = student.GradeId
    ,@studentAssessmentLevelId = student.AssessmentLevelGroupId
from [Education].[Student-Active] student
    inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
    inner join [Framework].[User-Active] users on users.ContactId = contact.Id
    inner join [Framework].[Session-Active] session on session.UserId = users.Id
        and session.Id = @SessionId

select
     book.Id as Id
    ,book.Title as Title
    ,book.Author as Author
    ,book.Summary as Summary
    ,book.CoverImageId as CoverImageId
    ,book.PrefaceBinderId as PrefaceBinderId
    ,coverImage.Id as CoverImageId
    ,coverImage.BlobId as CoverImageBlobId
    ,coverImage.Name as CoverImageName
    ,coverImage.Format as CoverImageFormat
    ,coverImage.Width as CoverImageWidth
    ,coverImage.Height as CoverImageHeight
    ,coverImage.Caption as CoverImageCaption
    ,convert(bit, case when exists (
        select 1
        from [Content].[Page-Active] page
            inner join [Content].[Binder-Active] binder on page.BinderId = binder.Id and binder.Id = book.PrefaceBinderId
        where page.StatusId = @ContentStatusComplete
        ) then 1 else 0 end) as HasPreface
    ,convert(bit, case when exists (
        select 1
        from [Education].[Chapter-Active] chapter
            inner join [Content].[Binder-Active] binder on chapter.BinderId = binder.Id
            inner join [Content].[Page-Active] page on page.BinderId = binder.Id
        where chapter.BookId = book.Id
            and page.StatusId = @ContentStatusComplete
        ) then 1 else 0 end) as HasContent
    ,convert(bit, case when exists (
        select 1
        from [Education].[Trifold-Active] trifold
            inner join [Content].[Binder-Active] binder on trifold.BinderId = binder.Id
            inner join [Content].[Page-Active] page on page.BinderId = binder.Id
        where trifold.BookId = book.Id
            and trifold.StatusId = @ContentStatusComplete
        ) then 1 else 0 end) as HasMap
from [Education].[Book-Active] book
    left join [Framework].[ImageFile-Active] coverImage on book.CoverImageId = coverImage.Id
where book.Id = @Id
    and book.StatusId = @ContentStatusComplete

select
     game.Id as Id
    ,game.BookId as BookId
    ,game.[Key] as [Key]
    ,game.Title as Title
    ,icon.CssClass as IconName
from [Education].[Game-Active] game
    left join [Framework].[Icon-Active] icon on game.IconId = icon.Id
where game.BookId = @Id
    and game.StatusId = @ContentStatusComplete
    and game.GradeId = @studentGradeId
    and game.AssessmentLevelId = @studentAssessmentLevelId
    and (game.RequiresAchievementId is null
        or exists (
            select Id
            from [Education].[StudentAchievement-Active]
            where StudentId = @studentId
                and AchievementId = game.RequiresAchievementId
        )
    )
order by game.[Key]

select
     module.Id as Id
    ,module.Title as Title
    ,icon.CssClass as IconName
    ,module.BookId as BookId
    ,module.StatusId as StatusId
    ,module.BinderId as BinderId
    ,module.Ordinal as Ordinal
from [Education].[Module-Active] module
    left join [Framework].[Icon-Active] icon on module.IconId = icon.Id
where module.BookId = @Id
    and module.StatusId = @ContentStatusComplete
    and (module.RequiresAchievementId is null
        or exists (
            select Id
            from [Education].[StudentAchievement-Active]
            where StudentId = @studentId
                and AchievementId = module.RequiresAchievementId
        )
    )