create proc [EducationRostering].[RosterRemovalApply] (
     @RosterRunId uniqueidentifier
    ,@sourceId uniqueidentifier
    ,@updatedBy uniqueidentifier
    ,@now datetimeoffset
) as

set nocount on

create table #student (StudentId uniqueidentifier not null primary key)

insert #student
select distinct link.LocalId
from [Education].[RosterRole] role
    inner join [Education].[RosterLink-Active] link
        on link.RosterSourceId = @sourceId and link.Kind = N'role' and link.ExternalId = role.ExternalId
    inner join [Education].[Student-Active] student on student.Id = link.LocalId
where role.RosterRunId = @RosterRunId
    and role.Role = N'student'

create table #remove (
     Kind nvarchar(25) not null
    ,ExternalId nvarchar(255) not null
    ,LocalId uniqueidentifier not null
    ,primary key (Kind, ExternalId)
)

insert #remove
select Kind, ExternalId, LocalId
from [Education].[RosterChange]
where RosterRunId = @RosterRunId
    and Action = N'remove'
    and LocalId is not null
    and Kind <> N'school'

declare @removedRoleUser table (Id uniqueidentifier not null primary key)

insert @removedRoleUser
select UserId
from (
    select student.UserId
    from #remove removed inner join [Education].[Student-Active] student on removed.Kind = N'role' and removed.LocalId = student.Id
    union
    select schoolContact.UserId
    from #remove removed inner join [Education].[SchoolContact-Active] schoolContact on removed.Kind = N'role' and removed.LocalId = schoolContact.Id
    union
    select districtContact.UserId
    from #remove removed inner join [Education].[DistrictContact-Active] districtContact on removed.Kind = N'role' and removed.LocalId = districtContact.Id
) [user]
where UserId is not null

update membership
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[ClassroomStudent] membership
    inner join #remove removed on removed.Kind = N'enrollment' and removed.LocalId = membership.Id
where membership.VersionOf = membership.Id
    and membership.IsDeleted = 0

update membership
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[ClassroomTeacher] membership
    inner join #remove removed on removed.Kind = N'enrollment' and removed.LocalId = membership.Id
where membership.VersionOf = membership.Id
    and membership.IsDeleted = 0

update schoolContact
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[SchoolContact] schoolContact
    inner join #remove removed on removed.Kind = N'role' and removed.LocalId = schoolContact.Id
where schoolContact.VersionOf = schoolContact.Id
    and schoolContact.IsDeleted = 0
    and not exists (
        select 1
        from [Education].[RosterLink-Active] retained
        where retained.RosterSourceId = @sourceId
            and retained.Kind = N'role'
            and retained.LocalId = schoolContact.Id
            and not exists (
                select 1 from #remove
                where Kind = N'role' and ExternalId = retained.ExternalId
            )
    )

update districtContact
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[DistrictContact] districtContact
    inner join #remove removed on removed.Kind = N'role' and removed.LocalId = districtContact.Id
where districtContact.VersionOf = districtContact.Id
    and districtContact.IsDeleted = 0
    and not exists (
        select 1
        from [Education].[RosterLink-Active] retained
        where retained.RosterSourceId = @sourceId
            and retained.Kind = N'role'
            and retained.LocalId = districtContact.Id
            and not exists (
                select 1 from #remove
                where Kind = N'role' and ExternalId = retained.ExternalId
            )
    )

update studentTable
set IsDeleted = 1
    ,StatusId = '6382a289-9ee0-4f4a-9f32-f31da517c81a'
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Student] studentTable
    inner join #remove removed on removed.Kind = N'role' and removed.LocalId = studentTable.Id
where studentTable.VersionOf = studentTable.Id
    and studentTable.IsDeleted = 0
    and not exists (select 1 from #student activeStudent where activeStudent.StudentId = studentTable.Id)

update classroom
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[Classroom] classroom
    inner join #remove removed on removed.Kind = N'class' and removed.LocalId = classroom.Id
where classroom.VersionOf = classroom.Id
    and classroom.IsDeleted = 0

update organization
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Organization] organization
    inner join [Education].[Classroom] classroom on classroom.OrganizationId = organization.Id
    inner join #remove removed on removed.Kind = N'class' and removed.LocalId = classroom.Id
where organization.VersionOf = organization.Id
    and organization.IsDeleted = 0

declare @removedIdentity table (Id uniqueidentifier not null primary key)

insert @removedIdentity
select distinct identityTable.Id
from #remove removed
    inner join [Framework].[User-Active] [user] on [user].ContactId = removed.LocalId
    inner join [Framework].[ExternalIdentityLink-Active] identityLink on identityLink.UserId = [user].Id
    inner join [Framework].[ExternalIdentity-Active] identityTable on identityTable.Id = identityLink.ExternalIdentityId
where removed.Kind = N'person'

update sessionAuth
set Revoked = coalesce(Revoked, @now)
    ,RevocationReason = coalesce(RevocationReason, N'roster-removed')
from [Framework].[SessionAuth] sessionAuth
    inner join @removedIdentity removed on removed.Id = sessionAuth.ExternalIdentityId

update sessionTable
set IsDeleted = 1
    ,Ended = coalesce(Ended, @now)
from [Framework].[Session] sessionTable
    inner join [Framework].[SessionAuth] sessionAuth on sessionAuth.SessionId = sessionTable.Id
    inner join @removedIdentity removed on removed.Id = sessionAuth.ExternalIdentityId
where sessionTable.IsDeleted = 0

update sessionAuth
set Revoked = coalesce(Revoked, @now)
    ,RevocationReason = coalesce(RevocationReason, N'roster-removed')
from [Framework].[SessionAuth] sessionAuth
    inner join [Framework].[Session] sessionTable on sessionAuth.SessionId = sessionTable.Id
    inner join @removedRoleUser removed on removed.Id = sessionTable.UserId

update sessionTable
set IsDeleted = 1
    ,Ended = coalesce(Ended, @now)
from [Framework].[Session] sessionTable
    inner join @removedRoleUser removed on removed.Id = sessionTable.UserId
where sessionTable.IsDeleted = 0

update identityLink
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[ExternalIdentityLink] identityLink
    inner join @removedIdentity removed on removed.Id = identityLink.ExternalIdentityId
where identityLink.IsDeleted = 0

update identityTable
set Enabled = 0
from [Framework].[ExternalIdentity] identityTable
    inner join @removedIdentity removed on removed.Id = identityTable.Id
where identityTable.IsDeleted = 0
    and identityTable.Enabled = 1

update [user]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[User] [user]
where [user].VersionOf = [user].Id
    and [user].IsDeleted = 0
    and [user].Username like N'RosterUser-%'
    and exists (select 1 from #remove removed where removed.Kind = N'person' and removed.LocalId = [user].ContactId)

update [user]
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[User] [user]
    inner join @removedRoleUser removed on removed.Id = [user].Id
where [user].VersionOf = [user].Id
    and [user].IsDeleted = 0
    and [user].Username like N'RosterUser-%'
    and not exists (select 1 from [Education].[Student-Active] student where student.UserId = [user].Id)
    and not exists (select 1 from [Education].[SchoolContact-Active] schoolContact where schoolContact.UserId = [user].Id)
    and not exists (select 1 from [Education].[DistrictContact-Active] districtContact where districtContact.UserId = [user].Id)

update contact
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Framework].[Contact] contact
    inner join #remove removed on removed.Kind = N'person' and removed.LocalId = contact.Id
where contact.VersionOf = contact.Id
    and contact.IsDeleted = 0
    and not exists (select 1 from [Education].[Student-Active] student where student.ContactId = contact.Id)
    and not exists (select 1 from [Education].[SchoolContact-Active] schoolContact where schoolContact.ContactId = contact.Id)
    and not exists (select 1 from [Education].[DistrictContact-Active] districtContact where districtContact.ContactId = contact.Id)

update link
set IsDeleted = 1
    ,Updated = @now
    ,UpdatedBy = @updatedBy
from [Education].[RosterLink] link
    inner join #remove removed on removed.Kind = link.Kind and removed.ExternalId = link.ExternalId
where link.VersionOf = link.Id
    and link.IsDeleted = 0
    and link.RosterSourceId = @sourceId