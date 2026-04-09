create proc [EducationPublisher].[AssessmentSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@Status Framework.IdList readonly
    ,@AvailableStartStart datetimeoffset(7)
    ,@AvailableStartEnd datetimeoffset(7)
    ,@AvailableEndStart datetimeoffset(7)
    ,@AvailableEndEnd datetimeoffset(7)
    ,@Grades Framework.IdList readonly
    ,@Categories Framework.IdList readonly
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1
declare @statusCount int = (select count(1) from @Status)
declare @gradesCount int = (select count(1) from @Grades)
declare @categoriesCount int = (select count(1) from @Categories)

;with AssessmentCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then assessment.Name
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then assessment.Name
                end desc,
                case when (@SortField = 'Start' and @SortAscending = 1)
                    then assessment.AvailableStart
                end asc,
                case when (@SortField = 'Start' and @SortAscending = 0)
                    then assessment.AvailableStart
                end desc,
                case when (@SortField = 'End' and @SortAscending = 1)
                    then assessment.AvailableEnd
                end asc,
                case when (@SortField = 'End' and @SortAscending = 0)
                    then assessment.AvailableEnd
                end desc,
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then assessment.AvailableStart
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then assessment.AvailableStart
                end desc,
                case when (@SortField = 'Name' and @SortAscending = 1)
                    then assessment.AvailableEnd
                end asc,
                case when (@SortField = 'Name' and @SortAscending = 0)
                    then assessment.AvailableEnd
                end desc,
                case when (@SortField = 'Start' and @SortAscending = 1)
                    then assessment.Name
                end asc,
                case when (@SortField = 'Start' and @SortAscending = 0)
                    then assessment.Name
                end desc,
                case when (@SortField = 'Start' and @SortAscending = 1)
                    then assessment.AvailableEnd
                end asc,
                case when (@SortField = 'Start' and @SortAscending = 0)
                    then assessment.AvailableEnd
                end desc,
                case when (@SortField = 'End' and @SortAscending = 1)
                    then assessment.Name
                end asc,
                case when (@SortField = 'End' and @SortAscending = 0)
                    then assessment.Name
                end desc,
                case when (@SortField = 'End' and @SortAscending = 1)
                    then assessment.AvailableStart
                end asc,
                case when (@SortField = 'End' and @SortAscending = 0)
                    then assessment.AvailableStart
                end desc,
                case when (@SortAscending = 1)
                    then assessment.Id
                end asc,
                case when (@SortAscending = 0)
                    then assessment.Id
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,assessment.Id
    from [Education].[Assessment-Active] assessment
        left join [Education].[ContentCategory-Active] category on assessment.CategoryId = category.Id
        inner join [Education].[Grade-Active] grade on assessment.GradeId = grade.Id
        left join [Framework].[ImageFile-Active] imageFile on assessment.ImageFileId = imageFile.Id
        inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
        inner join [Framework].[ContentStatus-Active] status on assessment.StatusId = status.Id
    where 1 = 1
        and organization.Id = @organizationId
        and (@SearchText is null
            or 1=1
        )
        and (@statusCount = 0 or assessment.StatusId in (select Id from @Status))
        and (@AvailableStartStart is null or assessment.AvailableStart >= @AvailableStartStart)
        and (@AvailableStartEnd is null or assessment.AvailableStart < @AvailableStartEnd)
        and (@AvailableEndStart is null or assessment.AvailableEnd >= @AvailableEndStart)
        and (@AvailableEndEnd is null or assessment.AvailableEnd < @AvailableEndEnd)
        and (@gradesCount = 0 or assessment.GradeId in (select Id from @Grades))
        and (@categoriesCount = 0 or assessment.CategoryId in (select Id from @Categories))
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,assessment.Id
    ,assessment.Name
    ,assessment.StatusId
    ,status.Name as StatusName
    ,assessment.GradeId
    ,grade.Name as GradeName
    ,assessment.AvailableStart
    ,assessment.AvailableEnd
    ,assessment.CategoryId
    ,category.Name as CategoryName
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
    ,(select count(1) from [Education].[VocabPart-Active] where AssessmentId = assessment.Id) as VocabPartCount
    ,(select count(1) from [Education].[ListenPart-Active] where AssessmentId = assessment.Id) as ListenPartCount
    ,(select count(1) from [Education].[ReadPart-Active] where AssessmentId = assessment.Id) as ReadPartCount
from AssessmentCte cte
    inner join [Education].[Assessment-Active] assessment on cte.Id = assessment.Id
    left join [Education].[ContentCategory-Active] category on assessment.CategoryId = category.Id
    inner join [Education].[Grade-Active] grade on assessment.GradeId = grade.Id
    left join [Framework].[ImageFile-Active] imageFile on assessment.ImageFileId = imageFile.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on assessment.StatusId = status.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)