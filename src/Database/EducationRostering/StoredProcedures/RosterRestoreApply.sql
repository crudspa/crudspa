create proc [EducationRostering].[RosterRestoreApply] (
     @RosterRunId uniqueidentifier
    ,@sourceId uniqueidentifier
    ,@updatedBy uniqueidentifier
    ,@now datetimeoffset
) as

set nocount on

create table #restore (
     RosterLinkId uniqueidentifier not null
    ,Kind nvarchar(25) not null
    ,ExternalId nvarchar(255) not null
    ,LocalId uniqueidentifier not null
    ,primary key (Kind, ExternalId)
)

;with staged as (
    select N'term' Kind, ExternalId from [Education].[RosterTerm] where RosterRunId = @RosterRunId and Status = N'active'
    union all select N'course', ExternalId from [Education].[RosterCourse] where RosterRunId = @RosterRunId and Status = N'active'
    union all select N'class', ExternalId from [Education].[RosterClass] where RosterRunId = @RosterRunId and Status = N'active'
    union all select N'person', ExternalId from [Education].[RosterPerson] where RosterRunId = @RosterRunId and Status = N'active'
    union all select N'role', ExternalId from [Education].[RosterRole] where RosterRunId = @RosterRunId
    union all select N'enrollment', ExternalId from [Education].[RosterEnrollment] where RosterRunId = @RosterRunId and Status = N'active'
), archived as (
    select link.Id RosterLinkId, link.Kind, link.ExternalId, link.LocalId
        ,row_number() over (partition by link.Kind, link.ExternalId order by link.Updated desc, link.Id) Ordinal
    from staged
        inner join [Education].[RosterLink] link
            on link.RosterSourceId = @sourceId
            and link.Kind = staged.Kind
            and link.ExternalId = staged.ExternalId
            and link.VersionOf = link.Id
            and link.IsDeleted = 1
    where not exists (
        select 1
        from [Education].[RosterLink-Active] activeLink
        where activeLink.RosterSourceId = @sourceId
            and activeLink.Kind = staged.Kind
            and activeLink.ExternalId = staged.ExternalId
    )
)
insert #restore
select RosterLinkId, Kind, ExternalId, LocalId
from archived
where Ordinal = 1

update contact
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Contact] contact
    inner join #restore restored on restored.Kind = N'person' and restored.LocalId = contact.Id
where contact.VersionOf = contact.Id
    and contact.IsDeleted = 1

update organization
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Organization] organization
    inner join [Education].[Classroom] classroom on classroom.OrganizationId = organization.Id
    inner join #restore restored on restored.Kind = N'class' and restored.LocalId = classroom.Id
where organization.VersionOf = organization.Id
    and organization.IsDeleted = 1

update classroom
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Classroom] classroom
    inner join #restore restored on restored.Kind = N'class' and restored.LocalId = classroom.Id
where classroom.VersionOf = classroom.Id
    and classroom.IsDeleted = 1

create table #role (
     LocalId uniqueidentifier not null
    ,[Role] nvarchar(25) not null
    ,primary key (LocalId, [Role])
)

insert #role
select distinct restored.LocalId, staged.Role
from #restore restored
    inner join [Education].[RosterRole] staged
        on staged.RosterRunId = @RosterRunId
        and staged.ExternalId = restored.ExternalId
where restored.Kind = N'role'

update student
set IsDeleted = 0
    ,StatusId = '9b3a7c55-130f-4e71-9538-1925f2962a4c'
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Student] student
    inner join #role restored on restored.Role = N'student' and restored.LocalId = student.Id
where student.VersionOf = student.Id
    and student.IsDeleted = 1

update schoolContact
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[SchoolContact] schoolContact
    inner join #role restored on restored.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact') and restored.LocalId = schoolContact.Id
where schoolContact.VersionOf = schoolContact.Id
    and schoolContact.IsDeleted = 1

update districtContact
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[DistrictContact] districtContact
    inner join #role restored on restored.Role = N'district-admin' and restored.LocalId = districtContact.Id
where districtContact.VersionOf = districtContact.Id
    and districtContact.IsDeleted = 1

update [user]
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[User] [user]
where [user].VersionOf = [user].Id
    and [user].IsDeleted = 1
    and [user].Username like N'RosterUser-%'
    and exists (
        select 1 from [Education].[Student] student inner join #role restored on restored.Role = N'student' and restored.LocalId = student.Id where student.UserId = [user].Id
        union all select 1 from [Education].[SchoolContact] schoolContact inner join #role restored on restored.Role in (N'teacher', N'staff', N'principal', N'literacy-facilitator', N'contact') and restored.LocalId = schoolContact.Id where schoolContact.UserId = [user].Id
        union all select 1 from [Education].[DistrictContact] districtContact inner join #role restored on restored.Role = N'district-admin' and restored.LocalId = districtContact.Id where districtContact.UserId = [user].Id
    )

update membership
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[ClassroomStudent] membership
    inner join #restore restored on restored.Kind = N'enrollment' and restored.LocalId = membership.Id
where membership.VersionOf = membership.Id
    and membership.IsDeleted = 1

update membership
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[ClassroomTeacher] membership
    inner join #restore restored on restored.Kind = N'enrollment' and restored.LocalId = membership.Id
where membership.VersionOf = membership.Id
    and membership.IsDeleted = 1

update link
set IsDeleted = 0
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[RosterLink] link
    inner join #restore restored on restored.RosterLinkId = link.Id
where link.VersionOf = link.Id
    and link.IsDeleted = 1
    and link.RosterSourceId = @sourceId