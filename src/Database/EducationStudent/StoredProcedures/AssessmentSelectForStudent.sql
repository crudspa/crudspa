create proc [EducationStudent].[AssessmentSelectForStudent] (
     @SessionId uniqueidentifier
) as

declare @ContentStatusComplete uniqueidentifier = '0296c1f0-7d72-42d3-b7c2-377f077e7b9c'
declare @now datetimeoffset = sysdatetimeoffset()

declare @studentId uniqueidentifier = (
    select student.Id
    from [Education].[Student-Active] student
        inner join [Framework].[Contact-Active] contact on student.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

select
    assessmentAssignment.Id as AssessmentAssignmentId
    ,assessmentAssignment.AssessmentId as AssessmentId
    ,assessmentAssignment.StudentId as StudentId
    ,assessmentAssignment.Assigned as Assigned
    ,assessmentAssignment.Started as Started
    ,assessmentAssignment.Completed as Completed
    ,assessmentAssignment.Terminated as Terminated
    ,assessment.Name as Name
    ,assessment.AvailableStart as AvailableStart
    ,assessment.AvailableEnd as AvailableEnd
    ,imageFile.Id as ImageFileId
    ,imageFile.BlobId as ImageFileBlobId
    ,imageFile.Name as ImageFileName
    ,imageFile.Format as ImageFileFormat
    ,imageFile.Width as ImageFileWidth
    ,imageFile.Height as ImageFileHeight
    ,imageFile.Caption as ImageFileCaption
from [Education].[AssessmentAssignment-Active] assessmentAssignment
    inner join [Education].[Assessment-Active] assessment on assessmentAssignment.AssessmentId = assessment.Id
    left join [Framework].[ImageFile-Active] imageFile on assessment.ImageFileId = imageFile.Id
where assessmentAssignment.StudentId = @StudentId
    and assessmentAssignment.Terminated is null
    and assessmentAssignment.StartAfter <= @now
    and assessmentAssignment.EndBefore >= @now
    and assessment.AvailableStart <= @now
    and assessment.AvailableEnd >= @now
    and assessment.StatusId = @ContentStatusComplete
order by assessmentAssignment.StartAfter
    ,assessmentAssignment.EndBefore
    ,assessment.AvailableStart
    ,assessment.AvailableEnd
    ,assessment.Name