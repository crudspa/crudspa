create proc [EducationRostering].[RosterClassMatchApply] (
     @RosterRunId uniqueidentifier
    ,@RosterSourceId uniqueidentifier
    ,@SchoolYearId uniqueidentifier
    ,@UpdatedBy uniqueidentifier
    ,@Now datetimeoffset
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

;with selected as (
    select candidate.ExternalId, candidate.ClassroomId
    from #candidate candidate
    where candidate.Method = case when exists (
        select 1 from #candidate identifier
        where identifier.ExternalId = candidate.ExternalId and identifier.Method = N'id'
    ) then N'id' else N'name' end
)
insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @Now, @UpdatedBy, @RosterSourceId, N'class', value.ExternalId, value.ClassroomId, value.SourceHash
from (
    select newid() Id, selected.ExternalId, selected.ClassroomId, staged.SourceHash
    from selected
        inner join [Education].[RosterClass] staged
            on staged.RosterRunId = @RosterRunId
            and staged.ExternalId = selected.ExternalId
    where 1 = (
        select count(distinct candidate.ClassroomId)
        from selected candidate
        where candidate.ExternalId = selected.ExternalId
    )
) value
where not exists (
    select 1
    from [Education].[RosterLink-Active]
    where RosterSourceId = @RosterSourceId
        and Kind = N'class'
        and ExternalId = value.ExternalId
)