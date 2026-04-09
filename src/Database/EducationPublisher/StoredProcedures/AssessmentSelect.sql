create proc [EducationPublisher].[AssessmentSelect] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

set nocount on

select
     assessment.Id
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
from [Education].[Assessment-Active] assessment
    left join [Education].[ContentCategory-Active] category on assessment.CategoryId = category.Id
    inner join [Education].[Grade-Active] grade on assessment.GradeId = grade.Id
    left join [Framework].[ImageFile-Active] imageFile on assessment.ImageFileId = imageFile.Id
    inner join [Framework].[Organization-Active] organization on assessment.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on assessment.StatusId = status.Id
where assessment.Id = @Id
    and organization.Id = @organizationId