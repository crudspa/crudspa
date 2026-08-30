create proc [EducationRostering].[RosterSchoolApply] (
     @RosterRunId uniqueidentifier
    ,@sourceId uniqueidentifier
    ,@districtId uniqueidentifier
    ,@updatedBy uniqueidentifier
    ,@now datetimeoffset
    ,@timeZoneId nvarchar(32)
    ,@schoolYearId uniqueidentifier
) as

set nocount on

create table #school (
     ExternalId nvarchar(255) not null primary key
    ,SchoolId uniqueidentifier not null
)

insert #school
select staged.ExternalId, coalesce(link.SchoolId, newid())
from [Education].[RosterSchool] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'school'
        and selected.ExternalId = staged.ExternalId
    left join [Education].[RosterSchoolLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.ExternalId = staged.ExternalId
        and link.Included = 1
where staged.RosterRunId = @RosterRunId
    and staged.Kind = N'school'
    and staged.Status = N'active'

insert [Framework].[Organization] (Id, VersionOf, Updated, UpdatedBy, Name, TimeZoneId)
select school.SchoolId, school.SchoolId, @now, @updatedBy, staged.Name, @timeZoneId
from #school school
    inner join [Education].[RosterSchool] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = school.ExternalId
where not exists (select 1 from [Framework].[Organization-Active] where Id = school.SchoolId)

insert [Education].[School] (Id, VersionOf, Updated, UpdatedBy, OrganizationId, DistrictId, [Key])
select school.SchoolId, school.SchoolId, @now, @updatedBy, school.SchoolId, @districtId, staged.ExternalId
from #school school
    inner join [Education].[RosterSchool] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = school.ExternalId
where not exists (select 1 from [Education].[School-Active] where Id = school.SchoolId)

update organization
set Name = staged.Name
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Organization] organization
    inner join [Education].[School-Active] school on school.OrganizationId = organization.Id
    inner join #school mapped on mapped.SchoolId = school.Id
    inner join [Education].[RosterSchool] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = mapped.ExternalId
where organization.VersionOf = organization.Id
    and organization.IsDeleted = 0
    and organization.Name <> staged.Name

insert [Education].[RosterSchoolLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, ExternalId, SchoolId, Included, SourceHash)
select value.Id, value.Id, @now, @updatedBy, @sourceId, value.ExternalId, value.SchoolId, 1, value.SourceHash
from (
    select newid() Id, school.ExternalId, school.SchoolId, staged.SourceHash
    from #school school
        inner join [Education].[RosterSchool] staged
            on staged.RosterRunId = @RosterRunId
            and staged.ExternalId = school.ExternalId
    where not exists (
        select 1
        from [Education].[RosterSchoolLink-Active]
        where RosterSourceId = @sourceId
            and ExternalId = school.ExternalId
    )
) value

create table #person (
     ExternalId nvarchar(255) not null primary key
    ,ContactId uniqueidentifier not null
)

insert #person
select staged.ExternalId, coalesce(link.LocalId, newid())
from [Education].[RosterPerson] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'person'
        and selected.ExternalId = staged.ExternalId
    left join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'person'
        and link.ExternalId = staged.ExternalId
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'

insert [Framework].[Contact] (Id, VersionOf, Updated, UpdatedBy, FirstName, LastName, TimeZoneId)
select person.ContactId, person.ContactId, @now, @updatedBy, staged.FirstName, staged.LastName, @timeZoneId
from #person person
    inner join [Education].[RosterPerson] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = person.ExternalId
where not exists (select 1 from [Framework].[Contact-Active] where Id = person.ContactId)

update contact
set FirstName = staged.FirstName
    ,LastName = staged.LastName
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Contact] contact
    inner join #person person on person.ContactId = contact.Id
    inner join [Education].[RosterPerson] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = person.ExternalId
where contact.VersionOf = contact.Id
    and contact.IsDeleted = 0
    and (isnull(contact.FirstName, N'') <> staged.FirstName or isnull(contact.LastName, N'') <> staged.LastName)

insert [Framework].[ContactEmail] (Id, VersionOf, Updated, UpdatedBy, ContactId, Email, Ordinal)
select value.Id, value.Id, @now, @updatedBy, value.ContactId, value.Email, 0
from (
    select newid() Id, person.ContactId, staged.Email
    from #person person
        inner join [Education].[RosterPerson] staged
            on staged.RosterRunId = @RosterRunId
            and staged.ExternalId = person.ExternalId
    where staged.Email is not null
        and not exists (
            select 1
            from [Framework].[ContactEmail-Active]
            where ContactId = person.ContactId
                and Ordinal = 0
        )
) value

update email
set Email = staged.Email
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[ContactEmail] email
    inner join #person person on person.ContactId = email.ContactId
    inner join [Education].[RosterPerson] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = person.ExternalId
where email.VersionOf = email.Id
    and email.IsDeleted = 0
    and email.Ordinal = 0
    and staged.Email is not null
    and email.Email <> staged.Email

insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @now, @updatedBy, @sourceId, N'person', value.ExternalId, value.ContactId, value.SourceHash
from (
    select newid() Id, person.ExternalId, person.ContactId, staged.SourceHash
    from #person person
        inner join [Education].[RosterPerson] staged
            on staged.RosterRunId = @RosterRunId
            and staged.ExternalId = person.ExternalId
    where not exists (
        select 1 from [Education].[RosterLink-Active]
        where RosterSourceId = @sourceId and Kind = N'person' and ExternalId = person.ExternalId
    )
) value

create table #class (
     ExternalId nvarchar(255) not null primary key
    ,ClassroomId uniqueidentifier not null
)

insert #class
select staged.ExternalId, coalesce(link.LocalId, newid())
from [Education].[RosterClass] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'class'
        and selected.ExternalId = staged.ExternalId
    left join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'class'
        and link.ExternalId = staged.ExternalId
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'

insert [Framework].[Organization] (Id, VersionOf, Updated, UpdatedBy, Name, TimeZoneId)
select class.ClassroomId, class.ClassroomId, @now, @updatedBy, staged.Name, @timeZoneId
from #class class
    inner join [Education].[RosterClass] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = class.ExternalId
where not exists (select 1 from [Framework].[Organization-Active] where Id = class.ClassroomId)

insert [Education].[Classroom] (Id, VersionOf, Updated, UpdatedBy, OrganizationId, SchoolId, GradeId, SchoolYearId, TypeId, SmallClassroom)
select class.ClassroomId, class.ClassroomId, @now, @updatedBy, class.ClassroomId, school.SchoolId, grade.Id, @schoolYearId
    ,'0ae9f9d3-5e48-4e82-b85a-4b5afe34ce93', isnull(staged.SmallClassroom, 0)
from #class class
    inner join [Education].[RosterClass] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = class.ExternalId
    inner join #school school on school.ExternalId = staged.SchoolExternalId
    left join [Education].[Grade-Active] grade on grade.Ordinal = try_convert(int, staged.Grade)
where not exists (select 1 from [Education].[Classroom-Active] where Id = class.ClassroomId)

insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @now, @updatedBy, @sourceId, N'class', value.ExternalId, value.ClassroomId, value.SourceHash
from (
    select newid() Id, class.ExternalId, class.ClassroomId, staged.SourceHash
    from #class class
        inner join [Education].[RosterClass] staged
            on staged.RosterRunId = @RosterRunId
            and staged.ExternalId = class.ExternalId
    where not exists (
        select 1 from [Education].[RosterLink-Active]
        where RosterSourceId = @sourceId and Kind = N'class' and ExternalId = class.ExternalId
    )
) value

update organization
set Name = staged.Name
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Organization] organization
    inner join [Education].[Classroom-Active] classroom on classroom.OrganizationId = organization.Id
    inner join #class mapped on mapped.ClassroomId = classroom.Id
    inner join [Education].[RosterClass] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = mapped.ExternalId
where organization.VersionOf = organization.Id
    and organization.IsDeleted = 0
    and organization.Name <> staged.Name

update classroom
set SchoolId = school.SchoolId
    ,GradeId = grade.Id
    ,SchoolYearId = @schoolYearId
    ,SmallClassroom = coalesce(staged.SmallClassroom, classroom.SmallClassroom)
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Classroom] classroom
    inner join #class mapped on mapped.ClassroomId = classroom.Id
    inner join [Education].[RosterClass] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = mapped.ExternalId
    inner join #school school on school.ExternalId = staged.SchoolExternalId
    left join [Education].[Grade-Active] grade on grade.Ordinal = try_convert(int, staged.Grade)
where classroom.VersionOf = classroom.Id
    and classroom.IsDeleted = 0
    and (classroom.SchoolId <> school.SchoolId
        or isnull(classroom.GradeId, '00000000-0000-0000-0000-000000000000') <> isnull(grade.Id, '00000000-0000-0000-0000-000000000000')
        or classroom.SchoolYearId <> @schoolYearId
        or (staged.SmallClassroom is not null and classroom.SmallClassroom <> staged.SmallClassroom))