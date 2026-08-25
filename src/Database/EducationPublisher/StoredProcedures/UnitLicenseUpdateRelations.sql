create proc [EducationPublisher].[UnitLicenseUpdateRelations] (
     @SessionId uniqueidentifier
    ,@Id uniqueidentifier
    ,@AllBooks bit
    ,@AllLessons bit
    ,@Books Framework.IdList readonly
    ,@Lessons Framework.IdList readonly
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @now datetimeoffset = sysdatetimeoffset()
declare @unitId uniqueidentifier = (
    select unitLicense.UnitId
    from [Education].[UnitLicense-Active] unitLicense
        inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
        inner join [Framework].[License-Active] license on license.Id = unitLicense.LicenseId
    where unitLicense.Id = @Id
        and unit.OwnerId = @organizationId
        and license.OwnerId = @organizationId
)

set nocount on
set xact_abort on
begin transaction


if @unitId is null
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end

if exists (
    select 1
    from @Books requestedBook
    where not exists (
        select 1
        from [Education].[UnitBook-Active] unitBook
        where unitBook.UnitId = @unitId
            and unitBook.BookId = requestedBook.Id
    )
)
begin
    rollback transaction
    raiserror('Every selected book must belong to the licensed unit', 16, 1)
    return
end

if exists (
    select 1
    from @Lessons requestedLesson
    where not exists (
        select 1
        from [Education].[Lesson-Active] lesson
        where lesson.UnitId = @unitId
            and lesson.Id = requestedLesson.Id
    )
)
begin
    rollback transaction
    raiserror('Every selected lesson must belong to the licensed unit', 16, 1)
    return
end

update [Education].[UnitLicense]
set  Id = @Id
    ,Updated = @now
    ,UpdatedBy = @SessionId
    ,AllBooks = @AllBooks
    ,AllLessons = @AllLessons
where Id = @Id

if (@AllBooks = 0)
begin
    update [Education].[UnitLicenseBook]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where UnitLicenseId = @Id
        and IsDeleted = 0
        and VersionOf = Id
        and not exists (select 1 from @Books where Id = BookId)

    insert [Education].[UnitLicenseBook] (Id, VersionOf, Updated, UpdatedBy, UnitLicenseId, BookId)
    select guid.NewId, guid.NewId, @now, @SessionId, @Id, Id
    from @Books
    cross apply (select newid() as NewId) guid
    where not exists (
        select 1 from [Education].[UnitLicenseBook-Active]
        where UnitLicenseId = @Id and BookId = Id
    )
end
else
begin
    update [Education].[UnitLicenseBook]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where UnitLicenseId = @Id
        and IsDeleted = 0
        and VersionOf = Id
end

if (@AllLessons = 0)
begin
    update [Education].[UnitLicenseLesson]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where UnitLicenseId = @Id
        and IsDeleted = 0
        and VersionOf = Id
        and not exists (select 1 from @Lessons where Id = LessonId)

    insert [Education].[UnitLicenseLesson] (Id, VersionOf, Updated, UpdatedBy, UnitLicenseId, LessonId)
    select guid.NewId, guid.NewId, @now, @SessionId, @Id, Id
    from @Lessons
    cross apply (select newid() as NewId) guid
    where not exists (
        select 1 from [Education].[UnitLicenseLesson-Active]
        where UnitLicenseId = @Id and LessonId = Id
    )
end
else
begin
    update [Education].[UnitLicenseLesson]
    set  IsDeleted = 1
        ,Updated = @now
        ,UpdatedBy = @SessionId
    where UnitLicenseId = @Id
        and IsDeleted = 0
        and VersionOf = Id
end

commit transaction