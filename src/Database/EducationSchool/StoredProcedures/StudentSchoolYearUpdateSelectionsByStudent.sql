create proc [EducationSchool].[StudentSchoolYearUpdateSelectionsByStudent] (
     @SessionId uniqueidentifier
    ,@StudentId uniqueidentifier
    ,@SchoolYears Framework.IdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

begin transaction

    update [Education].[StudentSchoolYear]
    set IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where StudentId = @StudentId
        and IsDeleted = 0
        and SchoolYearId not in (select Id from @SchoolYears)

    declare @NewStudentSchoolYearIds Framework.NewIdList

    insert @NewStudentSchoolYearIds
    select Id, newid()
    from @SchoolYears
    where Id not in (
        select SchoolYearId
        from [Education].[StudentSchoolYear-Active]
        where StudentId = @StudentId
    )

    insert [Education].[StudentSchoolYear] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,StudentId
        ,SchoolYearId
    )
    select
        [NewId]
        ,[NewId]
        ,@now
        ,@SessionId
        ,@StudentId
        ,[Id]
    from @NewStudentSchoolYearIds

commit transaction