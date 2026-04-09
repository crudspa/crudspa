create proc [EducationDistrict].[ReportMarkViewed] (
     @Id uniqueidentifier
    ,@SessionId uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

insert [Education].[ReportViewed] (
    Id
    ,ReportId
    ,Updated
    ,UpdatedBy
)
values (
    newid()
    ,@Id
    ,@now
    ,@SessionId
)