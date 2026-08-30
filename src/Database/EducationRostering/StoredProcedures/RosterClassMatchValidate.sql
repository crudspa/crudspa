create proc [EducationRostering].[RosterClassMatchValidate] (
     @RosterRunId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
    ,@SchoolYearId uniqueidentifier
) as

set nocount on

create table #candidate (
     ExternalId nvarchar(255) not null
    ,ClassroomId uniqueidentifier not null
    ,Method nvarchar(10) not null
    ,primary key (ExternalId, ClassroomId, Method)
)

insert #candidate
select distinct staged.ExternalId, classroom.Id, N'id'
from [Education].[RosterClass] staged
    inner join [Education].[RosterSchoolLink-Active] school
        on school.RosterSourceId = @RosterSourceId
        and school.ExternalId = staged.SchoolExternalId
        and school.Included = 1
    inner join [Education].[Classroom-Active] classroom
        on classroom.SchoolId = school.SchoolId
        and classroom.SchoolYearId = @SchoolYearId
        and classroom.ImportNum = try_convert(int, staged.SisId)
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'
    and try_convert(int, staged.SisId) is not null
    and not exists (
        select 1 from [Education].[RosterLink-Active]
        where RosterSourceId = @RosterSourceId
            and Kind = N'class'
            and ExternalId = staged.ExternalId
    )

insert #candidate
select distinct staged.ExternalId, classroom.Id, N'name'
from [Education].[RosterClass] staged
    inner join [Education].[RosterSchoolLink-Active] school
        on school.RosterSourceId = @RosterSourceId
        and school.ExternalId = staged.SchoolExternalId
        and school.Included = 1
    left join [Education].[Grade-Active] grade on grade.Ordinal = try_convert(int, staged.Grade)
    inner join [Education].[Classroom-Active] classroom
        on classroom.SchoolId = school.SchoolId
        and classroom.SchoolYearId = @SchoolYearId
        and isnull(classroom.GradeId, '00000000-0000-0000-0000-000000000000') = isnull(grade.Id, '00000000-0000-0000-0000-000000000000')
    inner join [Framework].[Organization-Active] organization
        on organization.Id = classroom.OrganizationId
        and lower(trim(organization.Name)) collate Latin1_General_100_BIN2 = lower(trim(staged.Name)) collate Latin1_General_100_BIN2
where staged.RosterRunId = @RosterRunId
    and staged.Status = N'active'
    and not exists (
        select 1 from [Education].[RosterLink-Active]
        where RosterSourceId = @RosterSourceId
            and Kind = N'class'
            and ExternalId = staged.ExternalId
    )

insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', staged.ExternalId, N'blocking', N'class-match-ambiguous'
    ,N'The source class matches more than one existing classroom in this district.'
from [Education].[RosterClass] staged
where staged.RosterRunId = @RosterRunId
    and (1 < (select count(distinct ClassroomId) from #candidate where ExternalId = staged.ExternalId and Method = N'id')
        or (not exists (select 1 from #candidate where ExternalId = staged.ExternalId and Method = N'id')
            and 1 < (select count(distinct ClassroomId) from #candidate where ExternalId = staged.ExternalId and Method = N'name'))
        or exists (
            select 1
            from #candidate identifier
                inner join #candidate name
                    on name.ExternalId = identifier.ExternalId
                    and name.Method = N'name'
                    and name.ClassroomId <> identifier.ClassroomId
            where identifier.ExternalId = staged.ExternalId
                and identifier.Method = N'id'
        ))

;with selected as (
    select candidate.ExternalId, candidate.ClassroomId
    from #candidate candidate
    where candidate.Method = case when exists (
        select 1 from #candidate identifier
        where identifier.ExternalId = candidate.ExternalId and identifier.Method = N'id'
    ) then N'id' else N'name' end
        and 1 = (
            select count(distinct selectedCandidate.ClassroomId)
            from #candidate selectedCandidate
            where selectedCandidate.ExternalId = candidate.ExternalId
                and selectedCandidate.Method = candidate.Method
        )
), duplicate as (
    select ClassroomId
    from selected
    group by ClassroomId
    having count(distinct ExternalId) > 1
)
insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', selected.ExternalId, N'blocking', N'class-match-reused'
    ,N'Multiple source classes resolve to the same existing classroom.'
from selected
    inner join duplicate on duplicate.ClassroomId = selected.ClassroomId

;with selected as (
    select candidate.ExternalId, candidate.ClassroomId
    from #candidate candidate
    where candidate.Method = case when exists (
        select 1 from #candidate identifier
        where identifier.ExternalId = candidate.ExternalId and identifier.Method = N'id'
    ) then N'id' else N'name' end
        and 1 = (
            select count(distinct selectedCandidate.ClassroomId)
            from #candidate selectedCandidate
            where selectedCandidate.ExternalId = candidate.ExternalId
                and selectedCandidate.Method = candidate.Method
        )
)
insert [Education].[RosterIssue] (Id, RosterRunId, Kind, ExternalId, Severity, Code, Detail)
select newid(), @RosterRunId, N'class', selected.ExternalId, N'blocking', N'class-match-owned'
    ,N'The existing classroom is already managed by another roster class.'
from selected
where exists (
    select 1
    from [Education].[RosterLink-Active] link
    where link.Kind = N'class'
        and link.LocalId = selected.ClassroomId
        and (link.RosterSourceId <> @RosterSourceId or link.ExternalId <> selected.ExternalId)
)