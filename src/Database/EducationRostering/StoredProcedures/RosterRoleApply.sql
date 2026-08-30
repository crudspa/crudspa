create proc [EducationRostering].[RosterRoleApply] (
     @RosterRunId uniqueidentifier
    ,@sourceId uniqueidentifier
    ,@organizationId uniqueidentifier
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
select staged.ExternalId, link.SchoolId
from [Education].[RosterSchool] staged
    inner join [Education].[RosterSchoolLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.ExternalId = staged.ExternalId
        and link.Included = 1
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'

create table #person (
     ExternalId nvarchar(255) not null primary key
    ,ContactId uniqueidentifier not null
)

insert #person
select staged.ExternalId, link.LocalId
from [Education].[RosterPerson] staged
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'person'
        and link.ExternalId = staged.ExternalId
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'

create table #student (
     ExternalId nvarchar(255) not null primary key
    ,StudentId uniqueidentifier not null
    ,FamilyId uniqueidentifier not null
    ,FamilyOrganizationId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,SchoolId uniqueidentifier not null
    ,SchoolOrganizationId uniqueidentifier not null
    ,GradeId uniqueidentifier not null
)

;with selected as (
    select role.PersonExternalId
        ,school.SchoolId
        ,grade.Id GradeId
        ,row_number() over (partition by role.PersonExternalId order by role.[Primary] desc, role.ExternalId) Ordinal
    from [Education].[RosterRole] role
        inner join #school school on school.ExternalId = role.SchoolExternalId
        inner join [Education].[Grade-Active] grade on grade.Ordinal = try_convert(int, role.Grade)
    where role.RosterRunId = @RosterRunId
        and role.Role = N'student'
)
insert #student
select selected.PersonExternalId
    ,coalesce(existing.Id, newid())
    ,coalesce(existing.FamilyId, newid())
    ,coalesce(family.OrganizationId, newid())
    ,coalesce(existing.UserId, newid())
    ,selected.SchoolId
    ,school.OrganizationId
    ,selected.GradeId
from selected
    inner join #person person on person.ExternalId = selected.PersonExternalId
    inner join [Education].[School-Active] school on school.Id = selected.SchoolId
    left join [Education].[Student-Active] existing on existing.ContactId = person.ContactId
    left join [Education].[Family-Active] family on family.Id = existing.FamilyId
where selected.Ordinal = 1

insert [Framework].[Organization] (Id, VersionOf, Updated, UpdatedBy, Name, TimeZoneId)
select student.FamilyOrganizationId, student.FamilyOrganizationId, @now, @updatedBy
    ,left(staged.LastName + N' (family)', 75), @timeZoneId
from #student student
    inner join [Education].[RosterPerson] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = student.ExternalId
where not exists (select 1 from [Framework].[Organization-Active] where Id = student.FamilyOrganizationId)

insert [Education].[Family] (Id, VersionOf, Updated, UpdatedBy, OrganizationId, SchoolId)
select FamilyId, FamilyId, @now, @updatedBy, FamilyOrganizationId, SchoolId
from #student student
where not exists (select 1 from [Education].[Family-Active] where Id = student.FamilyId)

;with selected as (
    select role.PersonExternalId, school.SchoolId
        ,row_number() over (partition by role.PersonExternalId order by role.[Primary] desc, role.ExternalId) Ordinal
    from [Education].[RosterRole] role
        inner join #school school on school.ExternalId = role.SchoolExternalId
    where role.RosterRunId = @RosterRunId
        and role.Role = N'student'
)
update family
set SchoolId = selected.SchoolId
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Family] family
    inner join [Education].[Student-Active] student on student.FamilyId = family.Id
    inner join [Education].[RosterLink-Active] personLink
        on personLink.RosterSourceId = @sourceId
        and personLink.Kind = N'person'
        and personLink.LocalId = student.ContactId
    inner join selected
        on selected.PersonExternalId = personLink.ExternalId
        and selected.Ordinal = 1
where family.VersionOf = family.Id
    and family.IsDeleted = 0
    and family.SchoolId <> selected.SchoolId
    and not exists (
        select 1 from [Education].[Student-Active] sibling
        where sibling.FamilyId = family.Id and sibling.Id <> student.Id
    )

insert [Framework].[User] (Id, VersionOf, Updated, UpdatedBy, ContactId, PortalId, OrganizationId, Username, ResetPassword)
select student.UserId, student.UserId, @now, @updatedBy, person.ContactId
    ,'ce747d25-1bfa-41b5-862d-cea01ea8a0ef', student.SchoolOrganizationId
    ,N'RosterUser-' + lower(convert(nvarchar(36), student.UserId)), 0
from #student student
    inner join #person person on person.ExternalId = student.ExternalId
where not exists (select 1 from [Framework].[User-Active] where Id = student.UserId)

insert [Education].[Student] (
     Id, VersionOf, Updated, UpdatedBy, ContactId, UserId, FamilyId, StatusId, GradeId, IdNumber
    ,AssessmentTypeGroupId, AssessmentLevelGroupId, ConditionGroupId, GoalSettingGroupId
    ,PersonalizationGroupId, ContentGroupId, AudioGenderId, ChallengeLevelId, MoreSample, TextSample
)
select student.StudentId, student.StudentId, @now, @updatedBy, person.ContactId, student.UserId, student.FamilyId
    ,'9b3a7c55-130f-4e71-9538-1925f2962a4c', student.GradeId, staged.SisId
    ,'945d1ca4-e458-4fc9-9656-5b31fb23459d'
    ,case staged.AssessmentLevel
        when N'low' then '45686b18-7fe9-4e6a-b665-d201f596a1cd'
        when N'high' then '9751eb86-73d3-46b8-80e5-5b27ce433312'
        else 'c3f45f06-f90a-4ffc-a16c-8ec1f22929c0'
    end
    ,'3befe15f-f053-4f26-826f-583dead12346', '7a20cf56-bbfe-408c-8208-f1636a7851cf'
    ,'49c37bc2-15e3-4afc-bc7f-d0846d460807', '2f742d69-63f7-4a05-bd13-15bc528ea55e'
    ,'5c9bce2e-fc32-4863-a143-00d10254f218', 'e2fa4f47-3587-4ff3-8dcf-008e7d1431be', 1, 1
from #student student
    inner join #person person on person.ExternalId = student.ExternalId
    inner join [Education].[RosterPerson] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = student.ExternalId
where not exists (select 1 from [Education].[Student-Active] where Id = student.StudentId)

update studentTable
set GradeId = student.GradeId
    ,IdNumber = staged.SisId
    ,StatusId = '9b3a7c55-130f-4e71-9538-1925f2962a4c'
    ,AssessmentLevelGroupId = case staged.AssessmentLevel
        when N'low' then '45686b18-7fe9-4e6a-b665-d201f596a1cd'
        when N'mid' then 'c3f45f06-f90a-4ffc-a16c-8ec1f22929c0'
        when N'high' then '9751eb86-73d3-46b8-80e5-5b27ce433312'
        else studentTable.AssessmentLevelGroupId
    end
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Student] studentTable
    inner join #student student on student.StudentId = studentTable.Id
    inner join [Education].[RosterPerson] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = student.ExternalId
where studentTable.VersionOf = studentTable.Id
    and studentTable.IsDeleted = 0
    and (studentTable.GradeId <> student.GradeId
        or isnull(studentTable.IdNumber, N'') <> isnull(staged.SisId, N'')
        or studentTable.StatusId <> '9b3a7c55-130f-4e71-9538-1925f2962a4c'
        or (staged.AssessmentLevel is not null and studentTable.AssessmentLevelGroupId <> case staged.AssessmentLevel
            when N'low' then '45686b18-7fe9-4e6a-b665-d201f596a1cd'
            when N'mid' then 'c3f45f06-f90a-4ffc-a16c-8ec1f22929c0'
            when N'high' then '9751eb86-73d3-46b8-80e5-5b27ce433312'
        end))

insert [Education].[StudentSchoolYear] (Id, VersionOf, Updated, UpdatedBy, StudentId, SchoolYearId)
select value.Id, value.Id, @now, @updatedBy, value.StudentId, @schoolYearId
from (
    select newid() Id, student.StudentId
    from #student student
    where not exists (
        select 1 from [Education].[StudentSchoolYear-Active]
        where StudentId = student.StudentId and SchoolYearId = @schoolYearId
    )
) value

create table #schoolUser (
     ExternalId nvarchar(255) not null primary key
    ,UserId uniqueidentifier not null
    ,OrganizationId uniqueidentifier not null
)

;with selected as (
    select role.PersonExternalId
        ,school.SchoolId
        ,schoolEntity.OrganizationId
        ,row_number() over (partition by role.PersonExternalId order by role.[Primary] desc, role.ExternalId) Ordinal
    from [Education].[RosterRole] role
        inner join #school school on school.ExternalId = role.SchoolExternalId
        inner join [Education].[School-Active] schoolEntity on schoolEntity.Id = school.SchoolId
    where role.RosterRunId = @RosterRunId
        and role.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact')
)
insert #schoolUser
select selected.PersonExternalId, coalesce(existing.UserId, adopted.UserId, newid()), selected.OrganizationId
from selected
    outer apply (
        select top 1 schoolContact.UserId
        from [Education].[RosterRole] role
            inner join [Education].[RosterLink-Active] link
                on link.RosterSourceId = @sourceId
                and link.Kind = N'role'
                and link.ExternalId = role.ExternalId
            inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.Id = link.LocalId
        where role.RosterRunId = @RosterRunId
            and role.PersonExternalId = selected.PersonExternalId
            and role.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact')
    ) existing
    outer apply (
        select top 1 schoolContact.UserId
        from [Education].[RosterLink-Active] personLink
            inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.ContactId = personLink.LocalId
            inner join [Education].[School-Active] school on school.Id = schoolContact.SchoolId and school.DistrictId = @districtId
        where personLink.RosterSourceId = @sourceId
            and personLink.Kind = N'person'
            and personLink.ExternalId = selected.PersonExternalId
        order by schoolContact.Id
    ) adopted
where selected.Ordinal = 1

insert [Framework].[User] (Id, VersionOf, Updated, UpdatedBy, ContactId, PortalId, OrganizationId, Username, ResetPassword)
select schoolUser.UserId, schoolUser.UserId, @now, @updatedBy, person.ContactId
    ,'c882bec5-cca6-4327-8f37-7729b2839b80', schoolUser.OrganizationId
    ,N'RosterUser-' + lower(convert(nvarchar(36), schoolUser.UserId)), 0
from #schoolUser schoolUser
    inner join #person person on person.ExternalId = schoolUser.ExternalId
where not exists (select 1 from [Framework].[User-Active] where Id = schoolUser.UserId)

create table #schoolContact (
     ExternalId nvarchar(255) not null primary key
    ,SchoolContactId uniqueidentifier not null
    ,PersonExternalId nvarchar(255) not null
)

create table #schoolPerson (
     PersonExternalId nvarchar(255) not null
    ,SchoolId uniqueidentifier not null
    ,SchoolContactId uniqueidentifier not null
    ,primary key (PersonExternalId, SchoolId)
)

;with wanted as (
    select distinct role.PersonExternalId, school.SchoolId
    from [Education].[RosterRole] role
        inner join #school school on school.ExternalId = role.SchoolExternalId
    where role.RosterRunId = @RosterRunId
        and role.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact')
)
insert #schoolPerson
select wanted.PersonExternalId, wanted.SchoolId, coalesce(existing.Id, adopted.Id, newid())
from wanted
    outer apply (
        select top 1 schoolContact.Id
        from [Education].[RosterRole] relatedRole
            inner join [Education].[RosterLink-Active] link
                on link.RosterSourceId = @sourceId
                and link.Kind = N'role'
                and link.ExternalId = relatedRole.ExternalId
            inner join [Education].[SchoolContact-Active] schoolContact
                on schoolContact.Id = link.LocalId
                and schoolContact.SchoolId = wanted.SchoolId
        where relatedRole.RosterRunId = @RosterRunId
            and relatedRole.PersonExternalId = wanted.PersonExternalId
        order by schoolContact.Id
    ) existing
    outer apply (
        select top 1 schoolContact.Id
        from [Education].[RosterLink-Active] personLink
            inner join [Education].[SchoolContact-Active] schoolContact
                on schoolContact.ContactId = personLink.LocalId
                and schoolContact.SchoolId = wanted.SchoolId
        where personLink.RosterSourceId = @sourceId
            and personLink.Kind = N'person'
            and personLink.ExternalId = wanted.PersonExternalId
        order by schoolContact.Id
    ) adopted

insert #schoolContact
select role.ExternalId, schoolPerson.SchoolContactId, role.PersonExternalId
from [Education].[RosterRole] role
    inner join #school school on school.ExternalId = role.SchoolExternalId
    inner join #schoolPerson schoolPerson
        on schoolPerson.PersonExternalId = role.PersonExternalId
        and schoolPerson.SchoolId = school.SchoolId
where role.RosterRunId = @RosterRunId
    and role.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'teacher-leader', N'contact')

insert [Education].[SchoolContact] (
    Id, VersionOf, Updated, UpdatedBy, SchoolId, ContactId, UserId, TitleId, IdNumber
)
select schoolPerson.SchoolContactId, schoolPerson.SchoolContactId, @now, @updatedBy, schoolPerson.SchoolId
    ,person.ContactId, schoolUser.UserId, '6ee60653-66df-49b9-9351-fcb9f0b9df68', stagedPerson.SisId
from #schoolPerson schoolPerson
    inner join #person person on person.ExternalId = schoolPerson.PersonExternalId
    inner join #schoolUser schoolUser on schoolUser.ExternalId = schoolPerson.PersonExternalId
    inner join [Education].[RosterPerson] stagedPerson
        on stagedPerson.RosterRunId = @RosterRunId
        and stagedPerson.ExternalId = schoolPerson.PersonExternalId
where not exists (select 1 from [Education].[SchoolContact-Active] where Id = schoolPerson.SchoolContactId)

insert [Education].[SchoolContactSchoolYear] (Id, VersionOf, Updated, UpdatedBy, SchoolContactId, SchoolYearId)
select value.Id, value.Id, @now, @updatedBy, value.SchoolContactId, @schoolYearId
from (
    select newid() Id, schoolPerson.SchoolContactId
    from #schoolPerson schoolPerson
    where not exists (
        select 1 from [Education].[SchoolContactSchoolYear-Active]
        where SchoolContactId = schoolPerson.SchoolContactId and SchoolYearId = @schoolYearId
    )
) value

create table #districtContact (
     ExternalId nvarchar(255) not null primary key
    ,DistrictContactId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,PersonExternalId nvarchar(255) not null
)

insert #districtContact
select role.ExternalId, coalesce(link.LocalId, adopted.Id, newid()), coalesce(existing.UserId, adopted.UserId, newid()), role.PersonExternalId
from [Education].[RosterRole] role
    left join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'role'
        and link.ExternalId = role.ExternalId
    left join [Education].[DistrictContact-Active] existing on existing.Id = link.LocalId
    outer apply (
        select top 1 districtContact.Id, districtContact.UserId
        from [Education].[RosterLink-Active] personLink
            inner join [Education].[DistrictContact-Active] districtContact on districtContact.ContactId = personLink.LocalId
        where personLink.RosterSourceId = @sourceId
            and personLink.Kind = N'person'
            and personLink.ExternalId = role.PersonExternalId
            and districtContact.DistrictId = @districtId
    ) adopted
where role.RosterRunId = @RosterRunId
    and role.Role = N'district-admin'

insert [Framework].[User] (Id, VersionOf, Updated, UpdatedBy, ContactId, PortalId, OrganizationId, Username, ResetPassword)
select districtContact.UserId, districtContact.UserId, @now, @updatedBy, person.ContactId
    ,'18da2a92-c650-42fb-8ff9-07c81ab5b9b2', @organizationId
    ,N'RosterUser-' + lower(convert(nvarchar(36), districtContact.UserId)), 0
from #districtContact districtContact
    inner join #person person on person.ExternalId = districtContact.PersonExternalId
where not exists (select 1 from [Framework].[User-Active] where Id = districtContact.UserId)

insert [Education].[DistrictContact] (Id, VersionOf, Updated, UpdatedBy, DistrictId, ContactId, UserId)
select districtContact.DistrictContactId, districtContact.DistrictContactId, @now, @updatedBy
    ,@districtId, person.ContactId, districtContact.UserId
from #districtContact districtContact
    inner join #person person on person.ExternalId = districtContact.PersonExternalId
where not exists (select 1 from [Education].[DistrictContact-Active] where Id = districtContact.DistrictContactId)

insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @now, @updatedBy, @sourceId, N'role', value.ExternalId, value.LocalId, value.SourceHash
from (
    select newid() Id, role.ExternalId
        ,case role.Role when N'student' then student.StudentId when N'district-admin' then districtContact.DistrictContactId else schoolContact.SchoolContactId end LocalId
        ,role.SourceHash
    from [Education].[RosterRole] role
        left join #student student on student.ExternalId = role.PersonExternalId
        left join #schoolContact schoolContact on schoolContact.ExternalId = role.ExternalId
        left join #districtContact districtContact on districtContact.ExternalId = role.ExternalId
    where role.RosterRunId = @RosterRunId
        and not exists (
            select 1 from [Education].[RosterLink-Active]
            where RosterSourceId = @sourceId and Kind = N'role' and ExternalId = role.ExternalId
        )
) value