create proc [EducationPublisher].[SchoolYearDelete] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[SchoolYear]
set IsDeleted = 1
where Id = @Id