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

set nocount on
set xact_abort on
begin transaction


if not exists (
    select 1
    from [Education].[UnitLicense-Active] unitLicense
        inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where unitLicense.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
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