create proc [EducationSchool].[SchoolContactUpdateRelations] (
     @SessionId uniqueidentifier
    ,@SchoolContactId uniqueidentifier
    ,@SchoolYears Framework.IdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @newSchoolContactSchoolYearIds Framework.NewIdList

begin transaction

    -- SchoolYear
    update [Education].[SchoolContactSchoolYear]
    set IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where SchoolContactId = @SchoolContactId
        and IsDeleted = 0
        and SchoolYearId not in (select Id from @SchoolYears)

    insert @newSchoolContactSchoolYearIds
    select Id, newid()
    from @SchoolYears
    where Id not in (
        select SchoolYearId
        from [Education].[SchoolContactSchoolYear-Active]
        where SchoolContactId = @SchoolContactId
    )

    insert [Education].[SchoolContactSchoolYear] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,SchoolContactId
        ,SchoolYearId
    )
    select
        [NewId]
        ,[NewId]
        ,@now
        ,@SessionId
        ,@SchoolContactId
        ,[Id]
    from @newSchoolContactSchoolYearIds

commit transaction