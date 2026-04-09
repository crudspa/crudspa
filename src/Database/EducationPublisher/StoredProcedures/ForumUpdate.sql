create proc [EducationPublisher].[ForumUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(75)
    ,@Description nvarchar(max)
    ,@BodyTemplateId uniqueidentifier
    ,@Pinned bit
    ,@DistrictId uniqueidentifier
    ,@SchoolId uniqueidentifier
    ,@InnovatorsOnly bit
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[Forum]
set
    Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,Name = @Name
    ,Description = @Description
    ,BodyTemplateId = @BodyTemplateId
    ,Pinned = @Pinned
    ,DistrictId = @DistrictId
    ,SchoolId = @SchoolId
    ,InnovatorsOnly = @InnovatorsOnly
where Id = @Id