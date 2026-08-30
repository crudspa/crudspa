create proc [EducationRostering].[RosterPersonMatchApply] (
     @RosterRunId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
    ,@OrganizationId uniqueidentifier
    ,@UpdatedBy uniqueidentifier
    ,@Now datetimeoffset
) as

set nocount on

create table #candidate (
     ExternalId nvarchar(255) not null
    ,ContactId uniqueidentifier not null
    ,Method nvarchar(10) not null
    ,primary key (ExternalId, ContactId, Method)
)

insert #candidate
select distinct person.ExternalId, student.ContactId, N'id'
from [Education].[RosterPerson] person
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @RosterSourceId) rosterSelection
        on rosterSelection.Kind = N'person'
        and rosterSelection.ExternalId = person.ExternalId
    inner join [Education].[RosterRole] role
        on role.RosterRunId = person.RosterRunId
        and role.PersonExternalId = person.ExternalId
        and role.Role = N'student'
    inner join [Education].[Student-Active] student on student.IdNumber = person.SisId
    inner join [Education].[Family-Active] family on family.Id = student.FamilyId
    inner join [Education].[School-Active] school on school.Id = family.SchoolId
    inner join [Education].[District-Active] district
        on district.Id = school.DistrictId
        and district.OrganizationId = @OrganizationId
where person.RosterRunId = @RosterRunId
    and person.Status = N'active'
    and person.SisId is not null

insert #candidate
select distinct person.ExternalId, schoolContact.ContactId, N'id'
from [Education].[RosterPerson] person
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @RosterSourceId) rosterSelection
        on rosterSelection.Kind = N'person'
        and rosterSelection.ExternalId = person.ExternalId
    inner join [Education].[RosterRole] role
        on role.RosterRunId = person.RosterRunId
        and role.PersonExternalId = person.ExternalId
        and role.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact')
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.IdNumber = person.SisId
    inner join [Education].[School-Active] school on school.Id = schoolContact.SchoolId
    inner join [Education].[District-Active] district
        on district.Id = school.DistrictId
        and district.OrganizationId = @OrganizationId
where person.RosterRunId = @RosterRunId
    and person.Status = N'active'
    and person.SisId is not null

insert #candidate
select distinct person.ExternalId, email.ContactId, N'email'
from [Education].[RosterPerson] person
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @RosterSourceId) rosterSelection
        on rosterSelection.Kind = N'person'
        and rosterSelection.ExternalId = person.ExternalId
    inner join [Framework].[ContactEmail-Active] email
        on lower(trim(email.Email)) = lower(trim(person.Email))
    inner join [Education].[SchoolContact-Active] schoolContact on schoolContact.ContactId = email.ContactId
    inner join [Education].[School-Active] school on school.Id = schoolContact.SchoolId
    inner join [Education].[District-Active] district
        on district.Id = school.DistrictId
        and district.OrganizationId = @OrganizationId
where person.RosterRunId = @RosterRunId
    and person.Status = N'active'
    and person.Email is not null
    and exists (
        select 1
        from [Education].[RosterRole]
        where RosterRunId = @RosterRunId
            and PersonExternalId = person.ExternalId
            and Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact')
    )

insert #candidate
select distinct person.ExternalId, email.ContactId, N'email'
from [Education].[RosterPerson] person
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @RosterSourceId) rosterSelection
        on rosterSelection.Kind = N'person'
        and rosterSelection.ExternalId = person.ExternalId
    inner join [Framework].[ContactEmail-Active] email
        on lower(trim(email.Email)) = lower(trim(person.Email))
    inner join [Education].[DistrictContact-Active] districtContact on districtContact.ContactId = email.ContactId
    inner join [Education].[District-Active] district
        on district.Id = districtContact.DistrictId
        and district.OrganizationId = @OrganizationId
where person.RosterRunId = @RosterRunId
    and person.Status = N'active'
    and person.Email is not null
    and exists (
        select 1
        from [Education].[RosterRole]
        where RosterRunId = @RosterRunId
            and PersonExternalId = person.ExternalId
            and Role = N'district-admin'
    )

;with selected as (
    select candidate.ExternalId, candidate.ContactId
    from #candidate candidate
    where candidate.Method = case when exists (
        select 1 from #candidate identifier
        where identifier.ExternalId = candidate.ExternalId and identifier.Method = N'id'
    ) then N'id' else N'email' end
)
insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @Now, @UpdatedBy, @RosterSourceId, N'person', value.ExternalId, value.ContactId, value.SourceHash
from (
    select newid() Id, selected.ExternalId, selected.ContactId, person.SourceHash
    from selected
        inner join [Education].[RosterPerson] person
            on person.RosterRunId = @RosterRunId
            and person.ExternalId = selected.ExternalId
    where 1 = (
        select count(distinct candidate.ContactId)
        from selected candidate
        where candidate.ExternalId = selected.ExternalId
    )
) value
where not exists (
    select 1
    from [Education].[RosterLink-Active]
    where RosterSourceId = @RosterSourceId
        and Kind = N'person'
        and ExternalId = value.ExternalId
)