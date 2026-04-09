create proc [EducationPublisher].[PostSelectWhereForForum] (
     @ForumId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Grades Framework.IdList readonly
    ,@Subjects Framework.IdList readonly
) as

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @gradesCount int = (select count(1) from @Grades)
declare @subjectsCount int = (select count(1) from @Subjects)

;with PostHierarchyCte as (
    select Id, ParentId, Id as RootId, 0 as Level
    from [Education].[Post-Active]
    where ParentId is null
    union all
    select p.Id, p.ParentId, h.RootId, h.Level + 1
    from [Education].[Post-Active] p
    inner join PostHierarchyCte h
        on p.ParentId = h.Id
),
CommentCountCte as (
    select RootId, count(*) as CommentCount
    from PostHierarchyCte
    where Level > 0
    group by RootId
),
PostCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Posted' and @SortAscending = 1)
                    then post.Posted
                end asc,
                case when (@SortField = 'Posted' and @SortAscending = 0)
                    then post.Posted
                end desc,
                case when (@SortAscending = 1)
                    then post.Id
                end asc,
                case when (@SortAscending = 0)
                    then post.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,post.Id
    from [Education].[Post-Active] post
    where 1 = 1
        and post.ForumId = @ForumId
        and (@SearchText is null
            or post.Body like '%' + @SearchText + '%'
        )
        and (@gradesCount = 0 or post.GradeId in (select Id from @Grades))
        and (@subjectsCount = 0 or post.SubjectId in (select Id from @Subjects))
)

select
    cte.RowNumber
    ,cte.TotalCount
    ,post.Id
    ,post.Pinned
    ,post.Body
    ,audio.Id as AudioId
    ,audio.BlobId as AudioBlobId
    ,audio.Name as AudioName
    ,audio.Format as AudioFormat
    ,audio.OptimizedStatus as AudioOptimizedStatus
    ,audio.OptimizedBlobId as AudioOptimizedBlobId
    ,audio.OptimizedFormat as AudioOptimizedFormat
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,pdf.Id as PdfId
    ,pdf.BlobId as PdfBlobId
    ,pdf.Name as PdfName
    ,pdf.Format as PdfFormat
    ,pdf.Description as PdfDescription
    ,video.Id as VideoId
    ,video.BlobId as VideoBlobId
    ,video.Name as VideoName
    ,video.Format as VideoFormat
    ,video.OptimizedStatus as VideoOptimizedStatus
    ,video.OptimizedBlobId as VideoOptimizedBlobId
    ,video.OptimizedFormat as VideoOptimizedFormat
    ,post.ById
    ,post.ByOrganizationName
    ,post.Posted
    ,post.Edited
    ,post.Type
    ,post.GradeId
    ,post.SubjectId
    ,trim(byTable.FirstName) as ByFirstName
    ,trim(byTable.LastName) as ByLastName
    ,grade.Name as GradeName
    ,subject.Name as SubjectName
    ,CommentCountCte.CommentCount as CommentCount
from PostCte cte
    inner join [Education].[Post-Active] post on cte.Id = post.Id
    inner join [Framework].[Contact-Active] byTable on post.ById = byTable.Id
    left join [Education].[Grade-Active] grade on post.GradeId = grade.Id
    left join [Education].[ClassroomType-Active] subject on post.SubjectId = subject.Id
    left join [Framework].[AudioFile-Active] audio on post.AudioId = audio.Id
    left join [Framework].[ImageFile-Active] image on post.ImageId = image.Id
    left join [Framework].[PdfFile-Active] pdf on post.PdfId = pdf.Id
    left join [Framework].[VideoFile-Active] video on post.VideoId = video.Id
    left join CommentCountCte on cte.Id = CommentCountCte.RootId
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc