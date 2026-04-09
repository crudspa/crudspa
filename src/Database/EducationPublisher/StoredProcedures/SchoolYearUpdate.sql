create proc [EducationPublisher].[SchoolYearUpdate] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@Name nvarchar(25)
    ,@Starts date
    ,@Ends date
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Education].[SchoolYear]
set
    Id = @Id
    ,Name = @Name
    ,Starts = @Starts
    ,Ends = @Ends
where Id = @Id