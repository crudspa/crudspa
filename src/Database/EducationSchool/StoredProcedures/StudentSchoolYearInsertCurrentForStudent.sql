create proc [EducationSchool].[StudentSchoolYearInsertCurrentForStudent] (
     @SessionId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@Id uniqueidentifier output
) as

set @Id = newid()

declare @now datetimeoffset = sysdatetimeoffset()
declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)

if not exists (select 1 from [Education].[StudentSchoolYear-Active] where StudentId = @StudentId and SchoolYearId = @schoolYearId)
begin
    begin transaction

        insert [Education].[StudentSchoolYear] (
            Id
            ,VersionOf
            ,Updated
            ,UpdatedBy
            ,StudentId
            ,SchoolYearId
        )
        values (
            @Id
            ,@Id
            ,@now
            ,@SessionId
            ,@StudentId
            ,@schoolYearId
        )

    commit transaction
end