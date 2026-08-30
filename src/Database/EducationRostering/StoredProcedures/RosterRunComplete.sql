create proc [EducationRostering].[RosterRunComplete] (
     @RosterRunId uniqueidentifier
    ,@Checkpoint nvarchar(500)
) as

set xact_abort on

declare @sourceId uniqueidentifier
    ,@kind nvarchar(25)
    ,@organizationId uniqueidentifier
    ,@schoolYearId uniqueidentifier

select
     @sourceId = rosterRun.RosterSourceId
    ,@kind = rosterRun.Kind
    ,@organizationId = source.OrganizationId
from [Education].[RosterRun] rosterRun with (updlock, holdlock)
    inner join [Education].[RosterSource-Active] source on rosterRun.RosterSourceId = source.Id
where rosterRun.Id = @RosterRunId
    and rosterRun.Status = N'started'

if @sourceId is null
    throw 50000, 'Roster run is unavailable or has already completed.', 1

exec [EducationRostering].[RosterPersonMatchValidate]
     @RosterRunId = @RosterRunId
    ,@RosterSourceId = @sourceId
    ,@OrganizationId = @organizationId

select top 1 @schoolYearId = Id
from [Education].[SchoolYear-Active]
where Starts <= convert(date, sysdatetimeoffset())
    and Ends > convert(date, sysdatetimeoffset())
order by Starts desc

exec [EducationRostering].[RosterClassMatchValidate]
     @RosterRunId = @RosterRunId
    ,@RosterSourceId = @sourceId
    ,@SchoolYearId = @schoolYearId

declare @bootstrapSchools bit = case when not exists (
    select 1
    from [Education].[School-Active] school
        inner join [Education].[District-Active] district on school.DistrictId = district.Id
    where district.OrganizationId = @organizationId
) and not exists (
    select 1
    from [Education].[RosterSchoolLink-Active]
    where RosterSourceId = @sourceId
) then 1 else 0 end

if not exists (select 1 from [Education].[RosterSchool] where RosterRunId = @RosterRunId)
    and not exists (select 1 from [Education].[RosterPerson] where RosterRunId = @RosterRunId)
    and not exists (select 1 from [Education].[RosterClass] where RosterRunId = @RosterRunId)
    and not exists (select 1 from [Education].[RosterEnrollment] where RosterRunId = @RosterRunId)
    insert [Education].[RosterIssue] (Id, RosterRunId, Kind, Severity, Code, Detail)
    values (newid(), @RosterRunId, N'source', N'blocking', N'empty-snapshot', N'The provider returned no schools, people, classes, or enrollments.')

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'school', school.ExternalId, N'blocking', N'school-unmapped', N'The source school is neither mapped nor explicitly excluded.'
from [Education].[RosterSchool] school
left join [Education].[RosterSchoolLink-Active] link
    on link.RosterSourceId = @sourceId
    and link.ExternalId = school.ExternalId
where school.RosterRunId = @RosterRunId
    and school.Kind = N'school'
    and link.Id is null
    and @bootstrapSchools = 0

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', class.ExternalId, N'blocking', N'school-missing', N'The class references a school that is absent from this run.'
from [Education].[RosterClass] class
left join [Education].[RosterSchool] school
    on school.RosterRunId = @RosterRunId
    and school.ExternalId = class.SchoolExternalId
where class.RosterRunId = @RosterRunId
    and school.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', class.ExternalId, N'blocking', N'course-missing', N'The class references a course that is absent from this run.'
from [Education].[RosterClass] class
left join [Education].[RosterCourse] course
    on course.RosterRunId = @RosterRunId
    and course.ExternalId = class.CourseExternalId
where class.RosterRunId = @RosterRunId
    and class.CourseExternalId is not null
    and course.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', class.ExternalId, N'blocking', N'term-missing', N'The class references a term that is absent from this run.'
from [Education].[RosterClass] class
left join [Education].[RosterTerm] term
    on term.RosterRunId = @RosterRunId
    and term.ExternalId = class.TermExternalId
where class.RosterRunId = @RosterRunId
    and class.TermExternalId is not null
    and term.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'role', role.ExternalId, N'blocking', N'person-missing', N'The role references a person that is absent from this run.'
from [Education].[RosterRole] role
left join [Education].[RosterPerson] person
    on person.RosterRunId = @RosterRunId
    and person.ExternalId = role.PersonExternalId
where role.RosterRunId = @RosterRunId
    and person.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'role', role.ExternalId, N'blocking', N'school-missing', N'The role references a school that is absent from this run.'
from [Education].[RosterRole] role
left join [Education].[RosterSchool] school
    on school.RosterRunId = @RosterRunId
    and school.ExternalId = role.SchoolExternalId
where role.RosterRunId = @RosterRunId
    and role.SchoolExternalId is not null
    and school.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'enrollment', enrollment.ExternalId, N'blocking', N'person-missing', N'The enrollment references a person that is absent from this run.'
from [Education].[RosterEnrollment] enrollment
left join [Education].[RosterPerson] person
    on person.RosterRunId = @RosterRunId
    and person.ExternalId = enrollment.PersonExternalId
where enrollment.RosterRunId = @RosterRunId
    and person.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'enrollment', enrollment.ExternalId, N'blocking', N'class-missing', N'The enrollment references a class that is absent from this run.'
from [Education].[RosterEnrollment] enrollment
left join [Education].[RosterClass] class
    on class.RosterRunId = @RosterRunId
    and class.ExternalId = enrollment.ClassExternalId
where enrollment.RosterRunId = @RosterRunId
    and class.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'enrollment', enrollment.ExternalId, N'blocking', N'school-missing', N'The enrollment references a school that is absent from this run.'
from [Education].[RosterEnrollment] enrollment
left join [Education].[RosterSchool] school
    on school.RosterRunId = @RosterRunId
    and school.ExternalId = enrollment.SchoolExternalId
where enrollment.RosterRunId = @RosterRunId
    and enrollment.SchoolExternalId is not null
    and school.Id is null

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'enrollment', enrollment.ExternalId, N'blocking', N'enrollment-duplicate'
    ,N'Multiple source enrollments assign the same person and role to the same class.'
from [Education].[RosterEnrollment] enrollment
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'enrollment'
        and selected.ExternalId = enrollment.ExternalId
where enrollment.RosterRunId = @RosterRunId
    and enrollment.Status = N'active'
    and 1 < (
        select count(*)
        from [Education].[RosterEnrollment] duplicate
        where duplicate.RosterRunId = enrollment.RosterRunId
            and duplicate.Status = N'active'
            and duplicate.ClassExternalId = enrollment.ClassExternalId
            and duplicate.PersonExternalId = enrollment.PersonExternalId
            and duplicate.Role = enrollment.Role
    )

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'school', school.ExternalId, N'blocking', N'name-too-long', N'The school name exceeds the configured limit.'
from [Education].[RosterSchool] school
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'school'
        and selected.ExternalId = school.ExternalId
where school.RosterRunId = @RosterRunId
    and school.Kind = N'school'
    and len(school.Name) > 75

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', class.ExternalId, N'blocking', N'name-too-long', N'The class name exceeds the configured limit.'
from [Education].[RosterClass] class
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'class'
        and selected.ExternalId = class.ExternalId
where class.RosterRunId = @RosterRunId
    and len(class.Name) > 75

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'person', person.ExternalId, N'blocking', N'person-field-too-long', N'The person name or email exceeds the configured limit.'
from [Education].[RosterPerson] person
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'person'
        and selected.ExternalId = person.ExternalId
where person.RosterRunId = @RosterRunId
    and (len(person.FirstName) > 75 or len(person.LastName) > 75 or len(person.Email) > 75)

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'role', role.ExternalId, N'blocking', N'grade-unmapped', N'The student grade is not supported by this application.'
from [Education].[RosterRole] role
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'role'
        and selected.ExternalId = role.ExternalId
where role.RosterRunId = @RosterRunId
    and role.Role = N'student'
    and not exists (
        select 1
        from [Education].[Grade-Active] grade
        where grade.Ordinal = try_convert(int, role.Grade)
    )

;with selected as (
    select role.ExternalId, role.PersonExternalId, school.SchoolId
        ,row_number() over (partition by role.PersonExternalId order by role.[Primary] desc, role.ExternalId) Ordinal
    from [Education].[RosterRole] role
        inner join [Education].[RosterSchoolLink-Active] school
            on school.RosterSourceId = @sourceId
            and school.ExternalId = role.SchoolExternalId
            and school.Included = 1
    where role.RosterRunId = @RosterRunId
        and role.Role = N'student'
)
insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'role', selected.ExternalId, N'blocking', N'family-school-conflict'
    ,N'The student moved schools, but their existing family contains another student.'
from selected
    inner join [Education].[RosterLink-Active] personLink
        on personLink.RosterSourceId = @sourceId
        and personLink.Kind = N'person'
        and personLink.ExternalId = selected.PersonExternalId
    inner join [Education].[Student-Active] student on student.ContactId = personLink.LocalId
    inner join [Education].[Family-Active] family on family.Id = student.FamilyId
where selected.Ordinal = 1
    and family.SchoolId <> selected.SchoolId
    and exists (
        select 1 from [Education].[Student-Active] sibling
        where sibling.FamilyId = family.Id and sibling.Id <> student.Id
    )

create table #item (
     Kind nvarchar(25) not null
    ,ExternalId nvarchar(255) not null
    ,LocalId uniqueidentifier null
    ,Action nvarchar(25) not null
    ,Severity nvarchar(25) not null
    ,Code nvarchar(75) not null
)

insert #item (Kind, ExternalId, LocalId, Action, Severity, Code)
select
     N'school'
    ,school.ExternalId
    ,link.SchoolId
    ,case when school.Status = N'inactive' and link.Included = 1 then N'remove' when link.Id is null and school.Kind = N'school' and @bootstrapSchools = 1 then N'add' when link.Id is null and school.Kind = N'school' then N'conflict' when link.Id is null or link.Included = 0 then N'excluded' when link.SourceHash = school.SourceHash then N'unchanged' else N'update' end
    ,case when school.Status = N'inactive' and link.Included = 1 then N'blocking' when link.Id is null and school.Kind = N'school' and @bootstrapSchools = 0 then N'blocking' else N'info' end
    ,case when school.Status = N'inactive' and link.Included = 1 then N'school-removal' when link.Id is null and school.Kind = N'school' and @bootstrapSchools = 1 then N'new' when link.Id is null and school.Kind = N'school' then N'school-unmapped' when link.Id is null then N'district-office' when link.Included = 0 then N'school-excluded' when link.SourceHash = school.SourceHash then N'unchanged' else N'changed' end
from [Education].[RosterSchool] school
left join [Education].[RosterSchoolLink-Active] link
    on link.RosterSourceId = @sourceId
    and link.ExternalId = school.ExternalId
where school.RosterRunId = @RosterRunId

insert #item (Kind, ExternalId, LocalId, Action, Severity, Code)
select staged.Kind, staged.ExternalId, link.LocalId
    ,case when selected.ExternalId is null and link.Id is not null then N'remove' when selected.ExternalId is null then N'excluded' when staged.Status = N'inactive' and link.Id is not null then N'remove' when staged.Status = N'inactive' then N'unchanged' when link.Id is null then N'add' when link.SourceHash = staged.SourceHash then N'unchanged' else N'update' end
    ,N'info'
    ,case when selected.ExternalId is null and link.Id is not null then N'outside-selection' when selected.ExternalId is null then N'school-excluded' when staged.Status = N'inactive' and link.Id is not null then N'inactive' when staged.Status = N'inactive' then N'unchanged' when link.Id is null then N'new' when link.SourceHash = staged.SourceHash then N'unchanged' else N'changed' end
from (
    select N'term' Kind, ExternalId, SourceHash, Status from [Education].[RosterTerm] where RosterRunId = @RosterRunId
    union all select N'course', ExternalId, SourceHash, Status from [Education].[RosterCourse] where RosterRunId = @RosterRunId
    union all select N'class', ExternalId, SourceHash, Status from [Education].[RosterClass] where RosterRunId = @RosterRunId
    union all select N'person', ExternalId, SourceHash, Status from [Education].[RosterPerson] where RosterRunId = @RosterRunId
    union all select N'role', ExternalId, SourceHash, N'active' from [Education].[RosterRole] where RosterRunId = @RosterRunId
    union all select N'enrollment', ExternalId, SourceHash, Status from [Education].[RosterEnrollment] where RosterRunId = @RosterRunId
) staged
left join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
    on selected.Kind = staged.Kind
    and selected.ExternalId = staged.ExternalId
left join [Education].[RosterLink-Active] link
    on link.RosterSourceId = @sourceId
    and link.Kind = staged.Kind
    and link.ExternalId = staged.ExternalId

if @kind = N'full'
begin
    insert #item (Kind, ExternalId, LocalId, Action, Severity, Code)
    select link.Kind, link.ExternalId, link.LocalId, N'remove', N'warning', N'absent-from-snapshot'
    from [Education].[RosterLink-Active] link
    where link.RosterSourceId = @sourceId
        and not exists (
            select 1
            from #item item
            where item.Kind = link.Kind
                and item.ExternalId = link.ExternalId
        )

    insert #item (Kind, ExternalId, LocalId, Action, Severity, Code)
    select N'school', link.ExternalId, link.SchoolId, N'remove', N'warning', N'absent-from-snapshot'
    from [Education].[RosterSchoolLink-Active] link
    where link.RosterSourceId = @sourceId
        and link.Included = 1
        and not exists (
            select 1
            from [Education].[RosterSchool] school
            where school.RosterRunId = @RosterRunId
                and school.ExternalId = link.ExternalId
        )
end

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'school', change.ExternalId, N'blocking', N'school-removal', N'School removal requires explicit reconciliation before apply.'
from #item change
where change.Kind = N'school'
    and change.Action = N'remove'

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'role', link.ExternalId, N'blocking', N'privileged-role-removal', N'District administrator removal requires explicit reconciliation before apply.'
from #item change
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'role'
        and link.ExternalId = change.ExternalId
    inner join [Education].[DistrictContact-Active] districtContact on districtContact.Id = link.LocalId
where change.Kind = N'role'
    and change.Action = N'remove'

insert [Education].[RosterChange] (Id, RosterRunId, Kind, ExternalId, LocalId, Action, Severity, Code)
select newid(), @RosterRunId, Kind, ExternalId, LocalId, Action, Severity, Code
from #item

declare @issueCount int = (select count(*) from [Education].[RosterIssue] where RosterRunId = @RosterRunId)
    ,@status nvarchar(25) = case when exists (
        select 1 from [Education].[RosterIssue] where RosterRunId = @RosterRunId and Severity = N'blocking'
    ) then N'blocked' else N'staged' end

update [Education].[RosterRun]
set Status = @status
    ,Completed = sysdatetimeoffset()
    ,[Checkpoint] = @Checkpoint
    ,SchoolCount = (select count(*) from [Education].[RosterSchool] where RosterRunId = @RosterRunId)
    ,TermCount = (select count(*) from [Education].[RosterTerm] where RosterRunId = @RosterRunId)
    ,CourseCount = (select count(*) from [Education].[RosterCourse] where RosterRunId = @RosterRunId)
    ,UserCount = (select count(*) from [Education].[RosterPerson] where RosterRunId = @RosterRunId)
    ,RoleCount = (select count(*) from [Education].[RosterRole] where RosterRunId = @RosterRunId)
    ,ClassCount = (select count(*) from [Education].[RosterClass] where RosterRunId = @RosterRunId)
    ,EnrollmentCount = (select count(*) from [Education].[RosterEnrollment] where RosterRunId = @RosterRunId)
    ,AddCount = (select count(*) from [Education].[RosterChange] where RosterRunId = @RosterRunId and Action = N'add')
    ,UpdateCount = (select count(*) from [Education].[RosterChange] where RosterRunId = @RosterRunId and Action = N'update')
    ,RemoveCount = (select count(*) from [Education].[RosterChange] where RosterRunId = @RosterRunId and Action = N'remove')
    ,IssueCount = @issueCount
where Id = @RosterRunId

exec [EducationRostering].[RosterRunSelect] @RosterRunId = @RosterRunId