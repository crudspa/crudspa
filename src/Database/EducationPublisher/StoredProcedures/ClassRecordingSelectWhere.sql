create proc [EducationPublisher].[ClassRecordingSelectWhere] (
     @SessionId uniqueidentifier
    ,@PageNumber int
    ,@PageSize int
    ,@SearchText nvarchar(50)
    ,@SortField nvarchar(50)
    ,@SortAscending bit
    ,@UploadedStart datetimeoffset(7)
    ,@UploadedEnd datetimeoffset(7)
) as

declare @publisherId uniqueidentifier = (
    select top 1 publisher.Id
    from [Education].[Publisher-Active] publisher
        inner join [Education].[PublisherContact-Active] publisherContact on publisherContact.PublisherId = publisher.Id
        inner join [Framework].[User-Active] userTable on publisherContact.ContactId = userTable.ContactId
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

declare @firstRecord int = (@PageSize * (@PageNumber - 1)) + 1
declare @lastRecord int = @firstRecord + @PageSize - 1

;with ClassRecordingCte
as (
    select
        row_number() over (
            order by
                case when (@SortField = 'Uploaded' and @SortAscending = 1)
                    then classRecording.Uploaded
                end asc
                ,case when (@SortField = 'Uploaded' and @SortAscending = 0)
                    then classRecording.Uploaded
                end desc
                ,case when (@SortField = 'Teacher' and @SortAscending = 1)
                    then contact.FirstName
                end asc
                ,case when (@SortField = 'Teacher' and @SortAscending = 1)
                    then contact.LastName
                end asc
                ,case when (@SortField = 'Teacher' and @SortAscending = 0)
                    then contact.FirstName
                end desc
                ,case when (@SortField = 'Teacher' and @SortAscending = 0)
                    then contact.LastName
                end desc
        ) as RowNumber
        ,count(*) over () as TotalCount
        ,classRecording.Id
    from [Education].[ClassRecording-Active] classRecording
        left join [Framework].[AudioFile-Active] audioFile on classRecording.AudioFileId = audioFile.Id
        inner join [Education].[ContentCategory-Active] category on classRecording.CategoryId = category.Id
        left join [Framework].[ImageFile-Active] image on classRecording.ImageId = image.Id
        inner join [Education].[SchoolContact-Active] schoolContact on classRecording.UploadedBy = schoolContact.Id
        inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
        inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
        inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
    where 1 = 1
        and district.PublisherId = @publisherId
        and (@SearchText is null
            or category.Name like '%' + @SearchText + '%'
            or classRecording.TeacherNotes like '%' + @SearchText + '%'
            or contact.FirstName like '%' + @SearchText + '%'
            or contact.LastName like '%' + @SearchText + '%'
            or organization.Name like '%' + @SearchText + '%'
        )
        and (@UploadedStart is null or classRecording.Uploaded >= @UploadedStart)
        and (@UploadedEnd is null or classRecording.Uploaded < @UploadedEnd)
)

select
     cte.RowNumber
    ,cte.TotalCount
    ,classRecording.Id
    ,classRecording.Uploaded
    ,audioFile.Id as AudioFileId
    ,audioFile.BlobId as AudioFileBlobId
    ,audioFile.Name as AudioFileName
    ,audioFile.Format as AudioFileFormat
    ,audioFile.OptimizedStatus as AudioFileOptimizedStatus
    ,audioFile.OptimizedBlobId as AudioFileOptimizedBlobId
    ,audioFile.OptimizedFormat as AudioFileOptimizedFormat
    ,image.Id as ImageId
    ,image.BlobId as ImageBlobId
    ,image.Name as ImageName
    ,image.Format as ImageFormat
    ,image.Width as ImageWidth
    ,image.Height as ImageHeight
    ,image.Caption as ImageCaption
    ,classRecording.CategoryId
    ,category.Name as CategoryName
    ,classRecording.Unit
    ,classRecording.Lesson
    ,classRecording.TeacherNotes
    ,schoolContact.Id as SchoolContactId
    ,contact.FirstName as ContactFirstName
    ,contact.LastName as ContactLastName
    ,organization.Name as OrganizationName
from ClassRecordingCte cte
    inner join [Education].[ClassRecording-Active] classRecording on cte.Id = classRecording.Id
    left join [Framework].[AudioFile-Active] audioFile on classRecording.AudioFileId = audioFile.Id
    inner join [Education].[ContentCategory-Active] category on classRecording.CategoryId = category.Id
    left join [Framework].[ImageFile-Active] image on classRecording.ImageId = image.Id
    inner join [Education].[SchoolContact-Active] schoolContact on classRecording.UploadedBy = schoolContact.Id
    inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
    inner join [Education].[School-Active] school on schoolContact.SchoolId = school.Id
    inner join [Framework].[Organization-Active] organization on school.OrganizationId = organization.Id
where cte.RowNumber >= @firstRecord and cte.RowNumber <= @lastRecord
order by cte.RowNumber asc
option (recompile)