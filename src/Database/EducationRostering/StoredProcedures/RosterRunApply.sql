create proc [EducationRostering].[RosterRunApply] (
    @RosterRunId uniqueidentifier
) as

set nocount on
set xact_abort on

declare @now datetimeoffset = sysdatetimeoffset()
    ,@sourceId uniqueidentifier
    ,@organizationId uniqueidentifier
    ,@districtId uniqueidentifier
    ,@provider nvarchar(75)
    ,@tenant nvarchar(255)
    ,@checkpoint nvarchar(500)
    ,@updatedBy uniqueidentifier
    ,@timeZoneId nvarchar(32)
    ,@schoolYearId uniqueidentifier

begin transaction

select
     @sourceId = run.RosterSourceId
    ,@organizationId = source.OrganizationId
    ,@provider = source.Provider
    ,@tenant = source.Tenant
    ,@checkpoint = run.[Checkpoint]
    ,@updatedBy = source.UpdatedBy
from [Education].[RosterRun] run with (updlock, holdlock)
    inner join [Education].[RosterSource] source with (updlock, holdlock)
        on run.RosterSourceId = source.Id
        and source.VersionOf = source.Id
        and source.IsDeleted = 0
        and source.Mode = N'authoritative'
where run.Id = @RosterRunId
    and run.Status = N'staged'

if @sourceId is null
    throw 50000, 'Only a staged run from an authoritative source can be applied.', 1

if exists (
    select 1
    from [Education].[RosterIssue]
    where RosterRunId = @RosterRunId
        and Severity = N'blocking'
)
    throw 50000, 'A roster run with blocking issues cannot be applied.', 1

if exists (
    select 1
    from [Education].[RosterRun] run
    where run.Id = @RosterRunId
        and run.Kind = N'full'
        and run.RemoveCount > 100
        and run.RemoveCount * 4 > (
            select count(*)
            from [Education].[RosterLink-Active]
            where RosterSourceId = @sourceId
        )
)
    throw 50000, 'The roster removal threshold requires review.', 1

select
     @districtId = district.Id
    ,@timeZoneId = organization.TimeZoneId
from [Education].[District-Active] district
    inner join [Framework].[Organization-Active] organization on district.OrganizationId = organization.Id
where district.OrganizationId = @organizationId

if @districtId is null
    throw 50000, 'The roster source is missing its district configuration.', 1

if @tenant is null and exists (
    select 1
    from [Education].[RosterPerson] person
        inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
            on selected.Kind = N'person'
            and selected.ExternalId = person.ExternalId
    where person.RosterRunId = @RosterRunId
        and person.AuthIssuer is not null
)
    throw 50000, 'The roster source is missing its tenant configuration.', 1

select top 1 @schoolYearId = Id
from [Education].[SchoolYear-Active]
where Starts <= convert(date, @now)
    and Ends > convert(date, @now)
order by Starts desc

if @schoolYearId is null
    throw 50000, 'No current school year is configured.', 1

exec [EducationRostering].[RosterPersonMatchValidate]
     @RosterRunId = @RosterRunId
    ,@RosterSourceId = @sourceId
    ,@OrganizationId = @organizationId

exec [EducationRostering].[RosterClassMatchValidate]
     @RosterRunId = @RosterRunId
    ,@RosterSourceId = @sourceId
    ,@SchoolYearId = @schoolYearId

if exists (
    select 1
    from [Education].[RosterIssue]
    where RosterRunId = @RosterRunId
        and Code in (N'person-match-ambiguous', N'person-match-reused', N'class-match-ambiguous', N'class-match-reused', N'class-match-owned')
)
    throw 50000, 'Roster identity matching changed after validation.', 1

exec [EducationRostering].[RosterPersonMatchApply]
     @RosterRunId = @RosterRunId
    ,@RosterSourceId = @sourceId
    ,@OrganizationId = @organizationId
    ,@UpdatedBy = @updatedBy
    ,@Now = @now

exec [EducationRostering].[RosterRestoreApply]
     @RosterRunId = @RosterRunId
    ,@sourceId = @sourceId
    ,@updatedBy = @updatedBy
    ,@now = @now

insert [Education].[RosterLink] (Id, VersionOf, Updated, UpdatedBy, RosterSourceId, Kind, ExternalId, LocalId, SourceHash)
select value.Id, value.Id, @now, @updatedBy, @sourceId, value.Kind, value.ExternalId, value.Id, value.SourceHash
from (
    select newid() Id, N'term' Kind, staged.ExternalId, staged.SourceHash
    from [Education].[RosterTerm] staged
        inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
            on selected.Kind = N'term' and selected.ExternalId = staged.ExternalId
    where staged.RosterRunId = @RosterRunId and staged.Status = N'active'
    union all
    select newid(), N'course', staged.ExternalId, staged.SourceHash
    from [Education].[RosterCourse] staged
        inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
            on selected.Kind = N'course' and selected.ExternalId = staged.ExternalId
    where staged.RosterRunId = @RosterRunId and staged.Status = N'active'
) value
where not exists (
    select 1 from [Education].[RosterLink-Active]
    where RosterSourceId = @sourceId and Kind = value.Kind and ExternalId = value.ExternalId
)

exec [EducationRostering].[RosterClassMatchApply]
     @RosterRunId = @RosterRunId
    ,@RosterSourceId = @sourceId
    ,@SchoolYearId = @schoolYearId
    ,@UpdatedBy = @updatedBy
    ,@Now = @now

exec [EducationRostering].[RosterSchoolApply]
     @RosterRunId = @RosterRunId
    ,@sourceId = @sourceId
    ,@districtId = @districtId
    ,@updatedBy = @updatedBy
    ,@now = @now
    ,@timeZoneId = @timeZoneId
    ,@schoolYearId = @schoolYearId

exec [EducationRostering].[RosterRoleApply]
     @RosterRunId = @RosterRunId
    ,@sourceId = @sourceId
    ,@organizationId = @organizationId
    ,@districtId = @districtId
    ,@updatedBy = @updatedBy
    ,@now = @now
    ,@timeZoneId = @timeZoneId
    ,@schoolYearId = @schoolYearId

exec [EducationRostering].[RosterMembershipApply]
     @RosterRunId = @RosterRunId
    ,@sourceId = @sourceId
    ,@provider = @provider
    ,@tenant = @tenant
    ,@updatedBy = @updatedBy
    ,@now = @now

exec [EducationRostering].[RosterRemovalApply]
     @RosterRunId = @RosterRunId
    ,@sourceId = @sourceId
    ,@updatedBy = @updatedBy
    ,@now = @now

update schoolLink
set SourceHash = staged.SourceHash
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[RosterSchoolLink] schoolLink
    inner join [Education].[RosterSchool] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = schoolLink.ExternalId
where schoolLink.VersionOf = schoolLink.Id
    and schoolLink.IsDeleted = 0
    and schoolLink.RosterSourceId = @sourceId
    and isnull(schoolLink.SourceHash, 0x) <> staged.SourceHash

update link
set SourceHash = staged.SourceHash
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[RosterLink] link
    inner join (
        select selected.Kind, selected.ExternalId, staged.SourceHash
        from [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
            inner join (
                select N'term' Kind, ExternalId, SourceHash from [Education].[RosterTerm] where RosterRunId = @RosterRunId
                union all select N'course', ExternalId, SourceHash from [Education].[RosterCourse] where RosterRunId = @RosterRunId
                union all select N'class', ExternalId, SourceHash from [Education].[RosterClass] where RosterRunId = @RosterRunId
                union all select N'person', ExternalId, SourceHash from [Education].[RosterPerson] where RosterRunId = @RosterRunId
                union all select N'role', ExternalId, SourceHash from [Education].[RosterRole] where RosterRunId = @RosterRunId
                union all select N'enrollment', ExternalId, SourceHash from [Education].[RosterEnrollment] where RosterRunId = @RosterRunId
            ) staged on staged.Kind = selected.Kind and staged.ExternalId = selected.ExternalId
    ) staged on staged.Kind = link.Kind and staged.ExternalId = link.ExternalId
where link.VersionOf = link.Id
    and link.IsDeleted = 0
    and link.RosterSourceId = @sourceId
    and isnull(link.SourceHash, 0x) <> staged.SourceHash

update [Education].[RosterRun]
set Status = N'applied'
where Id = @RosterRunId

update [Education].[RosterSource]
set [Checkpoint] = @checkpoint
    ,LastSucceeded = @now
    ,Updated = @now
    ,UpdatedBy = @updatedBy
where Id = @sourceId
    and VersionOf = Id
    and IsDeleted = 0

commit transaction

exec [EducationRostering].[RosterRunSelect] @RosterRunId = @RosterRunId