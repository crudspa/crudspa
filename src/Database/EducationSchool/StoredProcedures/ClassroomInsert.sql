create proc [EducationSchool].[ClassroomInsert] (
     @SessionId uniqueidentifier
    ,@Name nvarchar(75)
    ,@TypeId uniqueidentifier
    ,@GradeId uniqueidentifier
    ,@SmallClassroom bit
    ,@Id uniqueidentifier output
) as

declare @now datetimeoffset = sysdatetimeoffset()

declare @schoolId uniqueidentifier = (
    select top 1 school.Id
    from [Education].[School-Active] school
        inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.SchoolId = school.Id
        inner join [Framework].[Contact-Active] contact on schoolContact.ContactId = contact.Id
        inner join [Framework].[User-Active] userTable on userTable.ContactId = contact.Id
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

declare @schoolYearId uniqueidentifier = (select top 1 Id from [Education].[SchoolYear] where Starts <= @now and Ends > @now order by Starts desc)
declare @timeZoneId nvarchar(32) = (select top 1 TimeZoneId from [Framework].[Organization-Active] organization
                                inner join [Education].[School-Active] school on school.OrganizationId = organization.Id
                                where school.Id = @schoolId)

declare @organizationId uniqueidentifier = newid()
set @Id = newid()

begin transaction

    insert [Framework].[Organization] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,Name
        ,TimeZoneId
    )
    values (
        @organizationId
        ,@organizationId
        ,@now
        ,@SessionId
        ,@Name
        ,@TimeZoneId
    )

    insert [Education].[Classroom] (
        Id
        ,VersionOf
        ,Updated
        ,UpdatedBy
        ,OrganizationId
        ,TypeId
        ,SchoolId
        ,SchoolYearId
        ,GradeId
    ,SmallClassroom
    )
    values (
        @Id
        ,@Id
        ,@now
        ,@SessionId
        ,@organizationId
        ,@TypeId
        ,@schoolId
        ,@schoolYearId
        ,@GradeId
    ,@SmallClassroom
    )

commit transaction