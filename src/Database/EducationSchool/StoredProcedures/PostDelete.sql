create proc [EducationSchool].[PostDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[Post]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where Id = @Id