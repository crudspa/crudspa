create proc [EducationPublisher].[SchoolYearInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(25)
    ,@Starts date
    ,@Ends date
    ,@Id uniqueidentifier output
) as

set @Id = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[SchoolYear] (
    Id
    ,Name
    ,Starts
    ,Ends
)
values (
    @Id
    ,@Name
    ,@Starts
    ,@Ends
)