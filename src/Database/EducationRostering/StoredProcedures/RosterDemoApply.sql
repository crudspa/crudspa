create proc [EducationRostering].[RosterDemoApply] (
     @RosterRunId uniqueidentifier
    ,@Demos nvarchar(max)
) as

set nocount on

declare @now datetimeoffset = sysdatetimeoffset()
    ,@sourceId uniqueidentifier
    ,@updatedBy uniqueidentifier
    ,@schoolYearId uniqueidentifier

select @sourceId = source.Id
    ,@updatedBy = source.UpdatedBy
from [Education].[RosterRun] run
    inner join [Education].[RosterSource] source
        on source.Id = run.RosterSourceId
        and source.VersionOf = source.Id
        and source.IsDeleted = 0
where run.Id = @RosterRunId
    and run.Status = N'applied'

if @sourceId is null
    throw 50000, 'The applied roster run is unavailable.', 1

select top 1 @schoolYearId = Id
from [Education].[SchoolYear-Active]
where Starts <= convert(date, @now)
    and Ends > convert(date, @now)
order by Starts desc

declare @demo table (
     ClassroomId uniqueidentifier not null
    ,AssessmentLevel nvarchar(4) not null
    ,StudentId uniqueidentifier not null
    ,ContactId uniqueidentifier not null
    ,UserId uniqueidentifier not null
    ,FamilyId uniqueidentifier not null
    ,FamilyOrganizationId uniqueidentifier not null
    ,SecretCode nvarchar(75) not null
    ,primary key (ClassroomId, AssessmentLevel)
    ,unique (SecretCode)
)

create table #class (
    ClassroomId uniqueidentifier not null primary key
)

insert #class
select link.LocalId
from [Education].[RosterClass] staged
    inner join [EducationRostering].[RosterSelection](@RosterRunId, @sourceId) selected
        on selected.Kind = N'class'
        and selected.ExternalId = staged.ExternalId
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId
        and link.Kind = N'class'
        and link.ExternalId = staged.ExternalId
where staged.RosterRunId = @RosterRunId

create table #existing (
     ClassroomId uniqueidentifier not null
    ,AssessmentLevel nvarchar(4) not null
    ,primary key (ClassroomId, AssessmentLevel)
)

insert #existing
select distinct classroom.ClassroomId, contact.LastName
from #class classroom
    inner join [Education].[ClassroomStudent-Active] classroomStudent
        on classroomStudent.ClassroomId = classroom.ClassroomId
    inner join [Education].[Student-Active] student
        on student.Id = classroomStudent.StudentId
        and student.IsTestAccount = 1
    inner join [Framework].[Contact-Active] contact
        on contact.Id = student.ContactId
        and contact.FirstName = N'_Test'
        and contact.LastName in (N'Low', N'Mid', N'High')

insert @demo
select value.ClassroomId, value.AssessmentLevel, value.StudentId, value.ContactId, value.UserId
    ,value.FamilyId, value.FamilyOrganizationId, value.SecretCode
from openjson(@Demos) with (
     ClassroomId uniqueidentifier
    ,AssessmentLevel nvarchar(4)
    ,StudentId uniqueidentifier
    ,ContactId uniqueidentifier
    ,UserId uniqueidentifier
    ,FamilyId uniqueidentifier
    ,FamilyOrganizationId uniqueidentifier
    ,SecretCode nvarchar(75)
) value

if exists (select 1 from @demo where AssessmentLevel not in (N'Low', N'Mid', N'High'))
    or exists (
        select 1
        from @demo demo
            inner join [Education].[Student-Active] student on student.SecretCode = demo.SecretCode
    )
    or exists (
        select 1
        from @demo demo
            left join #class classroom on classroom.ClassroomId = demo.ClassroomId
        where classroom.ClassroomId is null
    )
    throw 50000, 'The demonstration account plan is invalid.', 1

delete demo
from @demo demo
where exists (select 1 from #existing where ClassroomId = demo.ClassroomId and AssessmentLevel = demo.AssessmentLevel)

insert [Framework].[Organization] (Id, VersionOf, Updated, UpdatedBy, Name, TimeZoneId)
select demo.FamilyOrganizationId, demo.FamilyOrganizationId, @now, @updatedBy
    ,demo.AssessmentLevel + N' (family)', schoolOrganization.TimeZoneId
from @demo demo
    inner join [Education].[Classroom-Active] classroom on classroom.Id = demo.ClassroomId
    inner join [Education].[School-Active] school on school.Id = classroom.SchoolId
    inner join [Framework].[Organization-Active] schoolOrganization on schoolOrganization.Id = school.OrganizationId

insert [Framework].[Contact] (Id, VersionOf, Updated, UpdatedBy, FirstName, LastName, TimeZoneId)
select demo.ContactId, demo.ContactId, @now, @updatedBy, N'_Test', demo.AssessmentLevel, schoolOrganization.TimeZoneId
from @demo demo
    inner join [Education].[Classroom-Active] classroom on classroom.Id = demo.ClassroomId
    inner join [Education].[School-Active] school on school.Id = classroom.SchoolId
    inner join [Framework].[Organization-Active] schoolOrganization on schoolOrganization.Id = school.OrganizationId

insert [Framework].[User] (Id, VersionOf, Updated, UpdatedBy, ContactId, PortalId, OrganizationId, Username, ResetPassword)
select demo.UserId, demo.UserId, @now, @updatedBy, demo.ContactId
    ,'ce747d25-1bfa-41b5-862d-cea01ea8a0ef', school.Id
    ,N'StudentUser-' + lower(convert(nvarchar(36), demo.StudentId)), 0
from @demo demo
    inner join [Education].[Classroom-Active] classroom on classroom.Id = demo.ClassroomId
    inner join [Education].[School-Active] schoolEntity on schoolEntity.Id = classroom.SchoolId
    inner join [Framework].[Organization-Active] school on school.Id = schoolEntity.OrganizationId

insert [Education].[Family] (Id, VersionOf, Updated, UpdatedBy, OrganizationId, SchoolId)
select demo.FamilyId, demo.FamilyId, @now, @updatedBy, demo.FamilyOrganizationId, classroom.SchoolId
from @demo demo
    inner join [Education].[Classroom-Active] classroom on classroom.Id = demo.ClassroomId

insert [Education].[Student] (
     Id, VersionOf, Updated, UpdatedBy, ContactId, UserId, FamilyId, StatusId, SecretCode, GradeId
    ,AssessmentTypeGroupId, AssessmentLevelGroupId, ConditionGroupId, GoalSettingGroupId
    ,PersonalizationGroupId, ContentGroupId, AudioGenderId, ChallengeLevelId
    ,MoreSample, TextSample, IsTestAccount
)
select demo.StudentId, demo.StudentId, @now, @updatedBy, demo.ContactId, demo.UserId, demo.FamilyId
    ,'9b3a7c55-130f-4e71-9538-1925f2962a4c', demo.SecretCode, classroom.GradeId
    ,'945d1ca4-e458-4fc9-9656-5b31fb23459d'
    ,case demo.AssessmentLevel when N'Low' then '45686b18-7fe9-4e6a-b665-d201f596a1cd' when N'High' then '9751eb86-73d3-46b8-80e5-5b27ce433312' else 'c3f45f06-f90a-4ffc-a16c-8ec1f22929c0' end
    ,'3befe15f-f053-4f26-826f-583dead12346', '7a20cf56-bbfe-408c-8208-f1636a7851cf'
    ,'49c37bc2-15e3-4afc-bc7f-d0846d460807', '2f742d69-63f7-4a05-bd13-15bc528ea55e'
    ,'5c9bce2e-fc32-4863-a143-00d10254f218', 'e2fa4f47-3587-4ff3-8dcf-008e7d1431be'
    ,1, 1, 1
from @demo demo
    inner join [Education].[Classroom-Active] classroom on classroom.Id = demo.ClassroomId

insert [Education].[StudentSchoolYear] (Id, VersionOf, Updated, UpdatedBy, StudentId, SchoolYearId)
select value.Id, value.Id, @now, @updatedBy, demo.StudentId, @schoolYearId
from @demo demo
    cross apply (select newid() Id) value

insert [Education].[ClassroomStudent] (Id, VersionOf, Updated, UpdatedBy, ClassroomId, StudentId)
select value.Id, value.Id, @now, @updatedBy, demo.ClassroomId, demo.StudentId
from @demo demo
    cross apply (select newid() Id) value