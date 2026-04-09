create proc [EducationPublisher].[ForumInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(75)
    ,@Description nvarchar(max)
    ,@BodyTemplateId uniqueidentifier
    ,@Pinned bit
    ,@DistrictId uniqueidentifier
    ,@SchoolId uniqueidentifier
    ,@InnovatorsOnly bit
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[Forum] (
    Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,Name
    ,Description
    ,BodyTemplateId
    ,Pinned
    ,DistrictId
    ,SchoolId
    ,InnovatorsOnly
)
values (
    @Id
    ,@Id
    ,@now
    ,@SessionId
    ,@Name
    ,@Description
    ,@BodyTemplateId
    ,@Pinned
    ,@DistrictId
    ,@SchoolId
    ,@InnovatorsOnly
)