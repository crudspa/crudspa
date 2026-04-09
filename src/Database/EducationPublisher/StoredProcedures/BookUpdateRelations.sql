create proc [EducationPublisher].[BookUpdateRelations] (
     @SessionId uniqueidentifier
    ,@BookId uniqueidentifier
    ,@Grades Framework.IdList readonly
    ,@SchoolYears Framework.IdList readonly
) as

declare @now datetimeoffset = sysdatetimeoffset()

set nocount on
set xact_abort on
begin transaction

-- Grade
update [Education].[BookGrade]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where BookId = @BookId
    and IsDeleted = 0
    and VersionOf = Id
    and not exists (
        select 1
        from @Grades
        where Id = GradeId
    )

insert [Education].[BookGrade] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BookId
    ,GradeId
)
select
     guid.NewId
    ,guid.NewId
    ,@now
    ,@SessionId
    ,@BookId
    ,Id
from @Grades
cross apply (select newid() as NewId) guid
where not exists (
    select 1
    from [Education].[BookGrade-Active]
    where BookId = @BookId
        and GradeId = Id
)

-- SchoolYear
update [Education].[BookSchoolYear]
set  IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @SessionId
where BookId = @BookId
    and IsDeleted = 0
    and VersionOf = Id
    and not exists (
        select 1
        from @SchoolYears
        where Id = SchoolYearId
    )

insert [Education].[BookSchoolYear] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,BookId
    ,SchoolYearId
)
select
     guid.NewId
    ,guid.NewId
    ,@now
    ,@SessionId
    ,@BookId
    ,Id
from @SchoolYears
cross apply (select newid() as NewId) guid
where not exists (
    select 1
    from [Education].[BookSchoolYear-Active]
    where BookId = @BookId
        and SchoolYearId = Id
)

commit transaction