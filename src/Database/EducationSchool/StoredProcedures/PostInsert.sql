create proc [EducationSchool].[PostInsert] (
     @SessionId uniqueidentifier
    ,@ForumId uniqueidentifier
    ,@ParentId uniqueidentifier
    ,@Pinned bit
    ,@Body nvarchar(max)
    ,@AudioId uniqueidentifier
    ,@ImageId uniqueidentifier
    ,@PdfId uniqueidentifier
    ,@VideoId uniqueidentifier
    ,@Type int
    ,@GradeId uniqueidentifier
    ,@SubjectId uniqueidentifier
    ,@Id uniqueidentifier output
) as

declare @contactId uniqueidentifier
declare @organizationName nvarchar(75)

select top 1
    @contactId = userTable.ContactId
    ,@organizationName = organization.Name
from [Framework].[User-Active] userTable
    inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    inner join [Framework].[Organization-Active] organization on organization.Id = userTable.OrganizationId
where session.Id = @SessionId

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[Post] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,ForumId
    ,ParentId
    ,Pinned
    ,Body
    ,AudioId
    ,ImageId
    ,PdfId
    ,VideoId
    ,Type
    ,GradeId
    ,SubjectId
    ,ById
    ,ByOrganizationName
    ,Posted
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@ForumId
    ,@ParentId
    ,@Pinned
    ,@Body
    ,@AudioId
    ,@ImageId
    ,@PdfId
    ,@VideoId
    ,@Type
    ,@GradeId
    ,@SubjectId
    ,@contactId
    ,@organizationName
    ,@now
)