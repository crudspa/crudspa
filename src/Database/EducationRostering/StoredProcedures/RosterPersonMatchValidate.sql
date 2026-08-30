create proc [EducationRostering].[RosterPersonMatchValidate] (
     @RosterRunId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
    ,@OrganizationId uniqueidentifier
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
    and not exists (
        select 1
        from [Education].[RosterLink-Active]
        where RosterSourceId = @RosterSourceId
            and Kind = N'person'
            and ExternalId = person.ExternalId
    )

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
    and not exists (
        select 1
        from [Education].[RosterLink-Active]
        where RosterSourceId = @RosterSourceId
            and Kind = N'person'
            and ExternalId = person.ExternalId
    )

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
    and not exists (
        select 1
        from [Education].[RosterLink-Active]
        where RosterSourceId = @RosterSourceId
            and Kind = N'person'
            and ExternalId = person.ExternalId
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
    and not exists (
        select 1
        from [Education].[RosterLink-Active]
        where RosterSourceId = @RosterSourceId
            and Kind = N'person'
            and ExternalId = person.ExternalId
    )

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'person', person.ExternalId, N'blocking', N'person-match-ambiguous'
    ,N'The source person matches more than one existing person in this district.'
from [Education].[RosterPerson] person
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @RosterSourceId) rosterSelection
        on rosterSelection.Kind = N'person'
        and rosterSelection.ExternalId = person.ExternalId
where person.RosterRunId = @RosterRunId
    and (1 < (select count(distinct ContactId) from #candidate where ExternalId = person.ExternalId and Method = N'id')
        or (not exists (select 1 from #candidate where ExternalId = person.ExternalId and Method = N'id')
            and 1 < (select count(distinct ContactId) from #candidate where ExternalId = person.ExternalId and Method = N'email'))
        or exists (
            select 1
            from #candidate identifier
                inner join #candidate email
                    on email.ExternalId = identifier.ExternalId
                    and email.Method = N'email'
                    and email.ContactId <> identifier.ContactId
            where identifier.ExternalId = person.ExternalId
                and identifier.Method = N'id'
        ))

;with selected as (
    select candidate.ExternalId, candidate.ContactId
    from #candidate candidate
    where candidate.Method = case when exists (
        select 1 from #candidate identifier
        where identifier.ExternalId = candidate.ExternalId and identifier.Method = N'id'
    ) then N'id' else N'email' end
        and 1 = (
            select count(distinct selectedCandidate.ContactId)
            from #candidate selectedCandidate
            where selectedCandidate.ExternalId = candidate.ExternalId
                and selectedCandidate.Method = candidate.Method
        )
), duplicate as (
    select ContactId
    from selected
    group by ContactId
    having count(distinct ExternalId) > 1
)
insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'person', selected.ExternalId, N'blocking', N'person-match-reused'
    ,N'Multiple source people resolve to the same existing person in this district.'
from selected
    inner join duplicate on duplicate.ContactId = selected.ContactId